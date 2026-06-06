#!/usr/bin/env node

import { existsSync, readFileSync, readdirSync, statSync } from "node:fs";
import { join, relative } from "node:path";
import { spawn } from "node:child_process";

const root = process.cwd();
const target = process.argv[2] ?? "all";

function fail(name, errors) {
  return { name, ok: false, errors };
}

function pass(name) {
  return { name, ok: true, errors: [] };
}

function read(path) {
  return readFileSync(join(root, path), "utf8");
}

function listFiles(dir, predicate = () => true) {
  const abs = join(root, dir);
  if (!existsSync(abs)) return [];
  const out = [];
  function walk(current) {
    for (const entry of readdirSync(current)) {
      const full = join(current, entry);
      const st = statSync(full);
      if (st.isDirectory()) walk(full);
      else if (predicate(full)) out.push(relative(root, full));
    }
  }
  walk(abs);
  return out;
}

async function runCommand(command, args) {
  return new Promise((resolve) => {
    const child = spawn(command, args, { stdio: "inherit", shell: false });
    child.on("exit", code => resolve(code ?? 1));
    child.on("error", () => resolve(1));
  });
}

async function checkAi() {
  const errors = [];
  if (!existsSync(join(root, "AGENTS.md"))) errors.push("AGENTS.md is missing.");
  if (!existsSync(join(root, "OpenQuestions.md"))) errors.push("OpenQuestions.md is missing.");
  if (!existsSync(join(root, "CLAUDE.md"))) errors.push("CLAUDE.md is missing.");
  else if (!read("CLAUDE.md").includes("AGENTS.md")) errors.push("CLAUDE.md must point to AGENTS.md.");
  const copilot = ".github/copilot-instructions.md";
  if (existsSync(join(root, copilot)) && !read(copilot).includes("AGENTS.md")) {
    errors.push(`${copilot} must point to AGENTS.md.`);
  }
  if (existsSync(join(root, "AGENTS.md")) && !read("AGENTS.md").includes("specs/")) {
    errors.push("AGENTS.md must mention specs/ as the spec-driven development workspace.");
  }
  return errors.length ? fail("ai instructions", errors) : pass("ai instructions");
}

async function checkOpenQuestions() {
  const errors = [];
  const file = "OpenQuestions.md";
  if (!existsSync(join(root, file))) {
    errors.push("OpenQuestions.md is missing.");
  } else {
    const content = read(file);
    for (const section of ["## Rules", "## Status values", "## Risk values", "## Open or inferred questions", "## Blockers", "## Answered or decided"]) {
      if (!content.includes(section)) errors.push(`OpenQuestions.md missing ${section}.`);
    }
    if (!content.includes("Proposed default")) errors.push("OpenQuestions.md must require proposed defaults for entries.");
  }
  return errors.length ? fail("open questions", errors) : pass("open questions");
}

async function checkSkills() {
  const errors = [];
  const files = listFiles(".agents/skills", f => f.endsWith("SKILL.md"));
  if (files.length === 0) errors.push("No .agents/skills/**/SKILL.md files found.");
  for (const file of files) {
    const content = read(file);
    if (!content.includes("name:")) errors.push(`${file} missing name metadata.`);
    if (!content.includes("description:")) errors.push(`${file} missing description metadata.`);
    if (!content.includes("## Validation")) errors.push(`${file} missing Validation section.`);
  }
  return errors.length ? fail("skills", errors) : pass("skills");
}

async function checkWorkflows() {
  const errors = [];
  const files = listFiles(".ai/workflows", f => f.endsWith(".md"));
  if (files.length === 0) errors.push("No .ai/workflows/*.md files found.");
  for (const file of files) {
    const content = read(file);
    for (const section of ["## Purpose", "## Steps", "## Required validation", "## Human approval"]) {
      if (!content.includes(section)) errors.push(`${file} missing ${section}.`);
    }
  }
  return errors.length ? fail("workflows", errors) : pass("workflows");
}

async function checkSpecs() {
  const errors = [];
  const required = [
    "specs/README.md",
    "specs/_template/spec.md",
    "specs/_template/plan.md",
    "specs/_template/tasks.md",
    "specs/_template/acceptance.md",
    "specs/_template/risks.md",
    "specs/_template/open-questions.md",
    "specs/0001-aegis-template-core/spec.md",
    "specs/0001-aegis-template-core/plan.md",
    "specs/0001-aegis-template-core/tasks.md",
    "specs/0001-aegis-template-core/acceptance.md",
    "specs/0001-aegis-template-core/risks.md",
    "specs/0001-aegis-template-core/open-questions.md"
  ];
  for (const file of required) {
    if (!existsSync(join(root, file))) errors.push(`${file} is missing.`);
  }

  const specFiles = listFiles("specs", f => f.endsWith("spec.md"));
  if (specFiles.length === 0) errors.push("No specs/**/spec.md files found.");
  for (const file of specFiles) {
    const content = read(file);
    for (const section of ["## Status", "## Problem", "## Goals", "## Non-goals"]) {
      if (!content.includes(section)) errors.push(`${file} missing ${section}.`);
    }
  }

  const specsRoot = join(root, "specs");
  if (existsSync(specsRoot)) {
    for (const entry of readdirSync(specsRoot)) {
      const full = join(specsRoot, entry);
      if (entry === "_template" || !statSync(full).isDirectory()) continue;
      for (const name of ["spec.md", "plan.md", "tasks.md", "acceptance.md", "risks.md", "open-questions.md"]) {
        const file = `specs/${entry}/${name}`;
        if (!existsSync(join(root, file))) errors.push(`${file} is missing.`);
      }
    }
  }

  return errors.length ? fail("specs", errors) : pass("specs");
}

async function checkModuleManifestTemplate() {
  const errors = [];
  const file = "templates/module-manifest/module.json";
  if (!existsSync(join(root, file))) {
    errors.push(`${file} is missing.`);
  } else {
    try {
      const manifest = JSON.parse(read(file));
      for (const property of ["name", "schema", "type", "owner", "dependencies", "publicContracts", "features", "rules"]) {
        if (!(property in manifest)) errors.push(`${file} missing ${property}.`);
      }
      if (manifest.rules && manifest.rules.allowCrossModuleDatabaseAccess !== false) {
        errors.push(`${file} must default allowCrossModuleDatabaseAccess to false.`);
      }
      if (manifest.rules && manifest.rules.allowInfrastructureReferences !== false) {
        errors.push(`${file} must default allowInfrastructureReferences to false.`);
      }
    } catch (error) {
      errors.push(`${file} is not valid JSON: ${error.message}`);
    }
  }
  return errors.length ? fail("module manifest template", errors) : pass("module manifest template");
}

async function checkDocs() {
  const required = [
    "docs/project-brief.md",
    "docs/architecture.md",
    "docs/cli-template-spec.md",
    "docs/implementation-plan.md",
    "docs/acceptance-criteria.md",
    "docs/git-plan.md",
    "docs/open-questions-protocol.md",
    "docs/open-questions-policy.md",
    "docs/competitive-analysis.md",
    "docs/getting-started/which-profile.md",
    "docs/ai-development/spec-driven-development.md",
    "docs/ai-development/ai-pr-protocol.md",
    "docs/architecture/module-manifest.md",
    "docs/adr/0012-use-spec-driven-development.md"
  ];
  const errors = required.filter(f => !existsSync(join(root, f))).map(f => `${f} is missing.`);
  return errors.length ? fail("docs", errors) : pass("docs");
}

async function checkSecurity() {
  const errors = [];
  const suspicious = listFiles(".", f => {
    const normalized = f.replaceAll("\\\\", "/");
    return normalized.endsWith("/.env") || normalized.includes("/secrets/") || normalized.endsWith("id_rsa");
  });
  for (const file of suspicious) errors.push(`Sensitive-looking file detected: ${file}`);
  return errors.length ? fail("security", errors) : pass("security");
}

async function checkDotnetAvailable() {
  const code = await runCommand("dotnet", ["--version"]);
  return code === 0 ? pass("dotnet available") : fail("dotnet available", ["dotnet CLI is not available."]);
}

async function checkTemplateSmokePlaceholder() {
  // This is a seed. Codex must replace this with real template smoke generation once templates exist.
  return pass("template smoke placeholder");
}

const groups = {
  ai: [checkAi, checkOpenQuestions, checkSkills, checkWorkflows],
  docs: [checkDocs, checkSpecs, checkModuleManifestTemplate],
  specs: [checkSpecs, checkModuleManifestTemplate],
  manifest: [checkModuleManifestTemplate],
  manifests: [checkModuleManifestTemplate],
  security: [checkSecurity],
  dotnet: [checkDotnetAvailable],
  "template-smoke": [checkTemplateSmokePlaceholder],
  all: [checkAi, checkOpenQuestions, checkSkills, checkWorkflows, checkDocs, checkSpecs, checkModuleManifestTemplate, checkSecurity]
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
    console.log(`✓ ${result.name}`);
  } else {
    failed = true;
    console.error(`✗ ${result.name}`);
    for (const error of result.errors) console.error(`  - ${error}`);
  }
}

process.exit(failed ? 1 : 0);
