#!/usr/bin/env node

import { existsSync, readFileSync, readdirSync, statSync } from "node:fs";
import { join, relative } from "node:path";
import { spawn } from "node:child_process";

const root = process.cwd();
const target = process.argv[2] ?? "all";
const options = {
  ai: "AegisAiValue",
  guardrails: "AegisGuardrailsValue",
  hooks: "AegisHooksValue",
  skills: "AegisSkillsValue",
  docs: "AegisDocsValue"
};

function pass(name) {
  return { name, ok: true, errors: [] };
}

function fail(name, errors) {
  return { name, ok: false, errors };
}

function exists(path) {
  return existsSync(join(root, path));
}

function read(path) {
  return readFileSync(join(root, path), "utf8");
}

function listFiles(dir, predicate = () => true) {
  const abs = join(root, dir);
  if (!existsSync(abs)) return [];
  const found = [];
  const ignoredDirectories = new Set([".git", "artifacts", "bin", "node_modules", "obj"]);

  function walk(current) {
    for (const entry of readdirSync(current)) {
      if (ignoredDirectories.has(entry)) continue;
      const full = join(current, entry);
      const st = statSync(full);
      if (st.isDirectory()) walk(full);
      else if (predicate(full)) found.push(relative(root, full).replaceAll("\\", "/"));
    }
  }

  walk(abs);
  return found;
}

function assertExists(errors, path, message) {
  if (!exists(path)) errors.push(message);
}

function assertMissing(errors, path, message) {
  if (exists(path)) errors.push(message);
}

function assertContains(errors, path, expected, message) {
  if (!exists(path)) {
    errors.push(`${message} Missing file: ${path}.`);
    return;
  }

  if (!read(path).includes(expected)) errors.push(message);
}

function readForbiddenActions(errors) {
  const path = ".ai/policies/forbidden-actions.yaml";
  if (!exists(path)) {
    errors.push("guardrails=strict with ai=enterprise must include forbidden action policy.");
    return [];
  }

  const entries = [];
  let current = null;
  for (const line of read(path).split(/\r?\n/)) {
    const item = line.match(/^\s*-\s+(pattern|path):\s*"([^"]+)"\s*$/);
    if (item) {
      current = { [item[1]]: item[2] };
      entries.push(current);
      continue;
    }

    const property = line.match(/^\s+(reason|requiresApproval|block):\s*(?:"([^"]+)"|(true|false))\s*$/);
    if (property && current) {
      current[property[1]] = property[2] ?? property[3] === "true";
    }
  }

  if (entries.length === 0) {
    errors.push("forbidden-actions.yaml must contain at least one forbidden entry.");
  }

  for (const entry of entries) {
    if (!entry.pattern && !entry.path) errors.push("Every forbidden action entry must define pattern or path.");
    if (!entry.reason && entry.pattern) errors.push(`Forbidden pattern ${entry.pattern} must include a reason.`);
    if (entry.block !== true && entry.requiresApproval !== true) {
      errors.push(`Forbidden entry ${entry.pattern ?? entry.path} must set block or requiresApproval.`);
    }
  }

  return entries;
}

function matchesForbiddenPath(pattern, file) {
  if (pattern === "**/.env") return file === ".env" || file.endsWith("/.env");
  if (pattern === "**/*secret*") return file.toLowerCase().includes("secret");
  if (pattern.endsWith("/**")) return file.startsWith(pattern.slice(0, -3));
  return file === pattern;
}

async function runCommand(command, args) {
  return new Promise(resolve => {
    const child = spawn(command, args, {
      cwd: root,
      env: process.env,
      stdio: "inherit",
      shell: false
    });
    child.on("exit", code => resolve(code ?? 1));
    child.on("error", () => resolve(1));
  });
}

function checkAi() {
  const errors = [];

  if (options.ai === "none") {
    for (const path of [
      "AGENTS.md",
      "CLAUDE.md",
      ".github/copilot-instructions.md",
      "OpenQuestions.md",
      ".ai",
      ".agents",
      "docs/ai-development",
      "specs"
    ]) {
      assertMissing(errors, path, `ai=none should not generate ${path}.`);
    }
    return errors.length ? fail("ai instructions", errors) : pass("ai instructions");
  }

  assertExists(errors, "AGENTS.md", "AI-enabled output must include AGENTS.md.");
  assertExists(errors, "CLAUDE.md", "AI-enabled output must include CLAUDE.md.");
  assertContains(errors, "CLAUDE.md", "AGENTS.md", "CLAUDE.md must point to AGENTS.md.");
  assertExists(errors, "OpenQuestions.md", "AI-enabled output must include OpenQuestions.md.");

  if (exists(".github/workflows/ci.yml")) {
    assertExists(errors, ".github/copilot-instructions.md", "GitHub-enabled AI output must include Copilot instructions.");
    assertContains(errors, ".github/copilot-instructions.md", "AGENTS.md", "Copilot instructions must point to AGENTS.md.");
  }

  if (options.ai === "enterprise") {
    for (const path of [".ai/policies", ".ai/workflows", ".ai/guardrails", ".ai/evals"]) {
      assertExists(errors, path, `ai=enterprise must include ${path}.`);
    }
  } else {
    for (const path of [".ai", ".agents", "specs"]) {
      assertMissing(errors, path, `ai=agents should not generate enterprise-only ${path}.`);
    }
  }

  return errors.length ? fail("ai instructions", errors) : pass("ai instructions");
}

function checkDocs() {
  const errors = [];
  for (const path of ["README.md", "docs/getting-started.md", "docs/architecture.md", "docs/development.md", "docs/module-manifest.md"]) {
    assertExists(errors, path, `docs output must include ${path}.`);
  }

  if (options.docs === "standard") {
    for (const path of ["docs/security.md", "docs/operations.md", "docs/adr", "docs/ai-development"]) {
      assertMissing(errors, path, `docs=standard should not generate expanded ${path}.`);
    }
  }

  if (options.docs === "full") {
    for (const path of ["docs/security.md", "docs/operations.md", "docs/adr/0001-modular-monolith.md"]) {
      assertExists(errors, path, `docs=full must include ${path}.`);
    }

    if (options.ai === "enterprise") {
      for (const path of [
        "docs/ai-development/agent-operating-model.md",
        "docs/ai-development/ai-pr-protocol.md",
        "docs/ai-development/guardrails.md",
        "docs/ai-development/skills.md",
        "docs/ai-development/spec-driven-development.md",
        "docs/ai-development/workflows.md"
      ]) {
        assertExists(errors, path, `docs=full with ai=enterprise must include ${path}.`);
      }
    } else if (options.ai === "agents") {
      for (const path of [
        "docs/ai-development/agent-operating-model.md",
        "docs/ai-development/ai-pr-protocol.md"
      ]) {
        assertExists(errors, path, `docs=full with ai=agents must include basic AI doc ${path}.`);
      }
      for (const path of [
        "docs/ai-development/guardrails.md",
        "docs/ai-development/skills.md",
        "docs/ai-development/spec-driven-development.md",
        "docs/ai-development/workflows.md"
      ]) {
        assertMissing(errors, path, `ai=agents should not include enterprise AI doc ${path}.`);
      }
    } else {
      assertMissing(errors, "docs/ai-development", "ai=none should not generate AI-specific docs.");
    }
  }

  return errors.length ? fail("docs", errors) : pass("docs");
}

function checkSecurity() {
  const errors = [];
  const suspicious = listFiles(".", file => {
    const normalized = file.toLowerCase();
    return normalized.endsWith("/.env") ||
      normalized.includes("/secrets/") ||
      normalized.endsWith("id_rsa") ||
      normalized.endsWith(".pem") ||
      normalized.endsWith(".pfx");
  });

  for (const file of suspicious) errors.push(`Sensitive-looking file detected: ${file}.`);

  const duplicateGuardrails = listFiles(".", file => {
    const normalized = file.toLowerCase();
    return (normalized.endsWith(".sh") || normalized.endsWith(".ps1")) &&
      (normalized.includes("guardrail") || normalized.includes("check"));
  });

  for (const file of duplicateGuardrails) {
    errors.push(`Do not duplicate guardrail check logic in shell scripts: ${file}.`);
  }

  return errors.length ? fail("security", errors) : pass("security");
}

function checkSkills() {
  const errors = [];
  const core = [
    "docs-writer",
    "dotnet-architecture-review",
    "dotnet-module",
    "dotnet-vertical-slice",
    "module-manifest",
    "spec-driven-feature"
  ];
  const enterprise = [
    ...core,
    "competitive-review",
    "efcore-migration-review",
    "guardrail-runner",
    "module-manifest-review",
    "openapi-contract-review",
    "security-review"
  ];
  const files = listFiles(".agents/skills", file => file.endsWith("SKILL.md"));
  const names = files.map(file => file.split("/").at(-2)).sort();

  if (options.ai !== "enterprise" || options.skills === "none") {
    if (files.length > 0) errors.push("skills=none or non-enterprise AI output should not include .agents/skills.");
    return errors.length ? fail("skills", errors) : pass("skills");
  }

  const expected = options.skills === "core" ? core : enterprise;
  for (const name of expected) {
    if (!names.includes(name)) errors.push(`skills=${options.skills} missing ${name}.`);
  }

  if (options.skills === "core") {
    for (const name of names) {
      if (!core.includes(name)) errors.push(`skills=core should not include enterprise-only ${name}.`);
    }
  }

  for (const file of files) {
    const content = read(file);
    for (const marker of ["name:", "description:", "## Validation"]) {
      if (!content.includes(marker)) errors.push(`${file} missing ${marker}.`);
    }
  }

  return errors.length ? fail("skills", errors) : pass("skills");
}

function checkWorkflows() {
  const errors = [];
  const files = listFiles(".ai/workflows", file => file.endsWith(".md"));

  if (options.ai !== "enterprise") {
    if (files.length > 0) errors.push("Non-enterprise AI output should not include .ai/workflows.");
    return errors.length ? fail("workflows", errors) : pass("workflows");
  }

  for (const name of ["create-module.md", "create-spec.md", "create-vertical-slice.md", "implement-spec.md", "pre-pr-review.md", "review-spec.md"]) {
    assertExists(errors, `.ai/workflows/${name}`, `ai=enterprise missing workflow ${name}.`);
  }

  for (const file of files) {
    const content = read(file);
    for (const marker of ["## Purpose", "## Steps", "## Required validation"]) {
      if (!content.includes(marker)) errors.push(`${file} missing ${marker}.`);
    }
  }

  return errors.length ? fail("workflows", errors) : pass("workflows");
}

function checkSpecs() {
  const errors = [];

  if (options.ai !== "enterprise") {
    assertMissing(errors, "specs", "Only ai=enterprise should generate specs.");
    return errors.length ? fail("specs", errors) : pass("specs");
  }

  for (const path of [
    "specs/README.md",
    "specs/_template/spec.md",
    "specs/_template/plan.md",
    "specs/_template/tasks.md",
    "specs/_template/acceptance.md",
    "specs/_template/risks.md",
    "specs/_template/open-questions.md"
  ]) {
    assertExists(errors, path, `ai=enterprise must include ${path}.`);
  }

  return errors.length ? fail("specs", errors) : pass("specs");
}

function checkModuleManifests() {
  const errors = [];
  const manifests = listFiles(".", file => file.replaceAll("\\", "/").endsWith("/module.json"));

  if (manifests.length === 0) {
    errors.push("No generated module.json manifests found under src/.");
  }

  for (const file of manifests) {
    try {
      const manifest = JSON.parse(read(file));
      for (const property of ["name", "schema", "type", "owner", "dependencies", "publicContracts", "features", "rules"]) {
        if (!(property in manifest)) errors.push(`${file} missing ${property}.`);
      }
      if (manifest.rules?.allowCrossModuleDatabaseAccess !== false) {
        errors.push(`${file} must set rules.allowCrossModuleDatabaseAccess to false.`);
      }
      if (manifest.rules?.allowInfrastructureReferences !== false) {
        errors.push(`${file} must set rules.allowInfrastructureReferences to false.`);
      }
    } catch (error) {
      errors.push(`${file} is not valid JSON: ${error.message}`);
    }
  }

  return errors.length ? fail("module manifests", errors) : pass("module manifests");
}

function checkStrict() {
  const errors = [];

  if (options.guardrails !== "strict") {
    assertMissing(errors, ".ai/policies/strict-mode.md", "standard/off guardrails should not include strict policy.");
    assertMissing(errors, ".ai/guardrails/strict-rules.md", "standard/off guardrails should not include strict rules.");
    return errors.length ? fail("strict guardrails", errors) : pass("strict guardrails");
  }

  if (options.ai === "enterprise") {
    assertExists(errors, ".ai/policies/strict-mode.md", "guardrails=strict with ai=enterprise must include strict policy.");
    assertExists(errors, ".ai/guardrails/strict-rules.md", "guardrails=strict with ai=enterprise must include strict rules.");
    assertContains(errors, ".ai/guardrails/strict-rules.md", "sensitive files", "strict rules must explain sensitive-file checks.");
    assertContains(errors, "AGENTS.md", "specs/", "strict enterprise AGENTS.md must mention specs/.");
    const forbiddenEntries = readForbiddenActions(errors);
    const sourceFiles = listFiles(".", file => !file.replaceAll("\\", "/").endsWith("/.ai/policies/forbidden-actions.yaml"));

    for (const entry of forbiddenEntries) {
      if (entry.pattern) {
        for (const file of sourceFiles) {
          if (read(file).includes(entry.pattern)) {
            errors.push(`Forbidden content pattern ${entry.pattern} detected in ${file}.`);
          }
        }
      }

      if (entry.path && entry.block === true) {
        for (const file of sourceFiles) {
          if (matchesForbiddenPath(entry.path, file)) {
            errors.push(`Blocked forbidden path detected: ${file}.`);
          }
        }
      }
    }
  } else {
    assertMissing(errors, ".ai", "Non-enterprise strict guardrails should not generate enterprise .ai assets.");
  }

  return errors.length ? fail("strict guardrails", errors) : pass("strict guardrails");
}

async function checkTemplateSmoke() {
  const solution = readdirSync(root).find(file => file.endsWith(".sln"));
  if (!solution) return fail("generated app smoke", ["No solution file was found."]);

  for (const args of [
    ["restore", solution],
    ["build", solution, "-c", "Release", "--no-restore"],
    ["test", solution, "-c", "Release", "--no-build"]
  ]) {
    const code = await runCommand("dotnet", args);
    if (code !== 0) return fail("generated app smoke", [`dotnet ${args[0]} failed.`]);
  }

  return pass("generated app smoke");
}

const groups = {
  ai: [checkAi],
  docs: [checkDocs],
  security: [checkSecurity],
  specs: [checkSpecs],
  skills: [checkSkills],
  workflows: [checkWorkflows],
  manifest: [checkModuleManifests],
  manifests: [checkModuleManifests],
  "template-smoke": [checkTemplateSmoke],
  all: [checkAi, checkDocs, checkSecurity, checkSkills, checkWorkflows, checkSpecs, checkStrict, checkModuleManifests]
};

const selected = groups[target];
if (!selected) {
  console.error(`Unknown target: ${target}`);
  process.exit(2);
}

let failed = false;
for (const check of selected) {
  const result = await check();
  if (result.ok) {
    console.log(`PASS ${result.name}`);
  } else {
    failed = true;
    console.error(`FAIL ${result.name}`);
    for (const error of result.errors) console.error(`  - ${error}`);
  }
}

process.exit(failed ? 1 : 0);
