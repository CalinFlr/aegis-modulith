#!/usr/bin/env node

import { existsSync, mkdirSync, readFileSync, readdirSync, rmSync, statSync } from "node:fs";
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
  const ignoredDirectories = new Set([".git", "artifacts", "bin", "obj"]);
  function walk(current) {
    for (const entry of readdirSync(current)) {
      if (ignoredDirectories.has(entry)) continue;
      const full = join(current, entry);
      const st = statSync(full);
      if (st.isDirectory()) walk(full);
      else if (predicate(full)) out.push(relative(root, full));
    }
  }
  walk(abs);
  return out;
}

async function runCommand(command, args, options = {}) {
  return new Promise((resolve) => {
    const child = spawn(command, args, {
      cwd: options.cwd ?? root,
      env: options.env ?? process.env,
      stdio: "inherit",
      shell: false
    });
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
    const normalized = f.replaceAll("\\", "/");
    return normalized.endsWith("/.env") || normalized.includes("/secrets/") || normalized.endsWith("id_rsa");
  });
  for (const file of suspicious) errors.push(`Sensitive-looking file detected: ${file}`);

  const duplicateGuardrailScripts = listFiles(".", f => {
    const normalized = f.replaceAll("\\", "/").toLowerCase();
    return (normalized.endsWith(".sh") || normalized.endsWith(".ps1")) &&
      (normalized.includes("guardrail") || normalized.includes("check"));
  });
  for (const file of duplicateGuardrailScripts) {
    errors.push(`Do not duplicate guardrail check logic in shell scripts: ${file}`);
  }

  return errors.length ? fail("security", errors) : pass("security");
}

async function checkDotnetAvailable() {
  const code = await runCommand("dotnet", ["--version"]);
  return code === 0 ? pass("dotnet available") : fail("dotnet available", ["dotnet CLI is not available."]);
}

async function checkCiWorkflows() {
  const required = [
    ".github/workflows/ci.yml",
    ".github/workflows/docs.yml",
    ".github/workflows/security.yml",
    ".github/workflows/specs.yml",
    ".github/workflows/guardrails.yml"
  ];

  const errors = required
    .filter(file => !existsSync(join(root, file)))
    .map(file => `${file} is missing.`);

  if (existsSync(join(root, ".github/workflows/ci.yml"))) {
    const ci = read(".github/workflows/ci.yml");
    for (const command of ["npm run check", "npm run template:smoke"]) {
      if (!ci.includes(command)) {
        errors.push(`.github/workflows/ci.yml must run ${command}.`);
      }
    }
  }

  return errors.length ? fail("ci workflows", errors) : pass("ci workflows");
}

async function checkTemplateSmoke() {
  const errors = [];
  const templateProject = "templates/Aegis.Modulith.Templates/Aegis.Modulith.Templates.csproj";
  if (!existsSync(join(root, templateProject))) {
    return fail("template smoke", [`${templateProject} is missing.`]);
  }

  const smokeRoot = join(root, "artifacts", "template-smoke");
  const packagesDir = join(smokeRoot, "packages");
  const generatedRoot = join(smokeRoot, "generated");
  const itemRoot = join(smokeRoot, "items");
  const dotnetHome = join(smokeRoot, "dotnet-home");

  rmSync(smokeRoot, { recursive: true, force: true });
  mkdirSync(packagesDir, { recursive: true });
  mkdirSync(generatedRoot, { recursive: true });
  mkdirSync(itemRoot, { recursive: true });
  mkdirSync(dotnetHome, { recursive: true });

  let code = await runCommand("dotnet", [
    "pack",
    templateProject,
    "-c",
    "Release",
    "-o",
    packagesDir
  ]);
  if (code !== 0) {
    return fail("template smoke", ["dotnet pack failed."]);
  }

  const nupkg = readdirSync(packagesDir)
    .filter(file => file.startsWith("Aegis.Modulith.Templates.") && file.endsWith(".nupkg"))
    .sort()
    .at(-1);

  if (!nupkg) {
    return fail("template smoke", ["Template package was not produced."]);
  }

  const smokeEnv = {
    ...process.env,
    DOTNET_CLI_HOME: dotnetHome,
    DOTNET_CLI_TELEMETRY_OPTOUT: "1",
    DOTNET_NOLOGO: "1"
  };

  code = await runCommand("dotnet", [
    "new",
    "install",
    join(packagesDir, nupkg),
    "--force"
  ], { env: smokeEnv });
  if (code !== 0) {
    return fail("template smoke", ["dotnet new install failed."]);
  }

  const matrix = [
    { id: "core-core", name: "Smoke.CoreCore", args: ["--profile", "core", "--mediator", "core"] },
    { id: "core-mediatr", name: "Smoke.CoreMediatR", args: ["--profile", "core", "--mediator", "mediatr"] },
    { id: "pro-core", name: "Smoke.ProCore", args: ["--profile", "pro", "--mediator", "core"] },
    { id: "pro-mediatr", name: "Smoke.ProMediatR", args: ["--profile", "pro", "--mediator", "mediatr"] },
    { id: "advanced-core", name: "Smoke.AdvancedCore", args: ["--profile", "advanced", "--mediator", "core"] },
    { id: "advanced-mediatr", name: "Smoke.AdvancedMediatR", args: ["--profile", "advanced", "--mediator", "mediatr"] },
    { id: "taskhub", name: "Aegis.TaskHub", args: ["--profile", "pro", "--sample", "taskhub"] },
    { id: "strict-enterprise", name: "Smoke.StrictEnterprise", args: ["--profile", "advanced", "--ai", "enterprise", "--guardrails", "strict", "--hooks", "lefthook"] }
  ];

  for (const variant of matrix) {
    const output = join(generatedRoot, variant.id);
    code = await runCommand("dotnet", [
      "new",
      "aegis-modulith",
      "-n",
      variant.name,
      ...variant.args,
      "-o",
      output,
      "--force"
    ], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`Generation failed for ${variant.id}.`]);
    }

    const solution = join(output, `${variant.name}.sln`);
    if (!existsSync(solution)) {
      return fail("template smoke", [`${variant.id} did not generate ${variant.name}.sln.`]);
    }

    for (const command of [
      ["restore", solution],
      ["build", solution, "-c", "Release", "--no-restore"],
      ["test", solution, "-c", "Release", "--no-build"]
    ]) {
      code = await runCommand("dotnet", command, { env: smokeEnv });
      if (code !== 0) {
        return fail("template smoke", [`dotnet ${command[0]} failed for ${variant.id}.`]);
      }
    }

    if (variant.id === "taskhub") {
      for (const moduleName of ["Projects", "Tasks", "Notifications", "Audit"]) {
        const manifest = join(output, "src", `${variant.name}.Modules`, "Modules", moduleName, "module.json");
        if (!existsSync(manifest)) {
          errors.push(`TaskHub sample missing ${moduleName} module manifest.`);
        }
      }

      const starterManifest = join(output, "src", `${variant.name}.Modules`, "Modules", "WorkItems", "module.json");
      if (existsSync(starterManifest)) {
        errors.push("TaskHub sample should not include the starter WorkItems module.");
      }
    }

    if (variant.id === "core-core") {
      const starterManifest = join(output, "src", `${variant.name}.Modules`, "Modules", "WorkItems", "module.json");
      const projectManifest = join(output, "src", `${variant.name}.Modules`, "Modules", "Projects", "module.json");
      if (!existsSync(starterManifest)) {
        errors.push("Core sample-none variant should include the starter WorkItems module.");
      }
      if (existsSync(projectManifest)) {
        errors.push("Core sample-none variant should not include TaskHub Projects module.");
      }
      if (existsSync(join(output, "lefthook.yml"))) {
        errors.push("hooks=none variant should not include lefthook.yml.");
      }
    }

    if (variant.id === "strict-enterprise" && !existsSync(join(output, "lefthook.yml"))) {
      errors.push("hooks=lefthook variant should include lefthook.yml.");
    }
  }

  const itemChecks = [
    {
      id: "module",
      args: ["new", "aegis-module", "-n", "Billing", "--schema", "billing", "-o", join(itemRoot, "module")],
      project: join(itemRoot, "module", "Billing.csproj"),
      required: [join(itemRoot, "module", "module.json")]
    },
    {
      id: "slice",
      args: ["new", "aegis-slice", "-n", "CreateInvoice", "--module", "Billing", "--kind", "command", "-o", join(itemRoot, "slice")],
      required: [join(itemRoot, "slice", "CreateInvoiceHandler.cs")]
    },
    {
      id: "event",
      args: ["new", "aegis-event", "-n", "InvoiceIssued", "--module", "Billing", "--scope", "integration", "-o", join(itemRoot, "event")],
      required: [join(itemRoot, "event", "InvoiceIssued.cs")]
    },
    {
      id: "worker",
      args: ["new", "aegis-worker", "-n", "BillingOutboxDispatcher", "--module", "Billing", "-o", join(itemRoot, "worker")],
      project: join(itemRoot, "worker", "BillingOutboxDispatcher.csproj"),
      required: [join(itemRoot, "worker", "Worker.cs")]
    }
  ];

  for (const item of itemChecks) {
    code = await runCommand("dotnet", [...item.args, "--force"], { env: smokeEnv });
    if (code !== 0) {
      return fail("template smoke", [`Item template generation failed for ${item.id}.`]);
    }

    for (const required of item.required) {
      if (!existsSync(required)) {
        errors.push(`${item.id} item missing ${required}.`);
      }
    }

    if (item.project) {
      code = await runCommand("dotnet", ["build", item.project, "-c", "Release"], { env: smokeEnv });
      if (code !== 0) {
        return fail("template smoke", [`Item template build failed for ${item.id}.`]);
      }
    }
  }

  return errors.length ? fail("template smoke", errors) : pass("template smoke");
}

const groups = {
  ai: [checkAi, checkOpenQuestions, checkSkills, checkWorkflows],
  docs: [checkDocs, checkSpecs, checkModuleManifestTemplate],
  specs: [checkSpecs, checkModuleManifestTemplate],
  manifest: [checkModuleManifestTemplate],
  manifests: [checkModuleManifestTemplate],
  security: [checkSecurity],
  dotnet: [checkDotnetAvailable],
  "template-smoke": [checkTemplateSmoke],
  all: [checkAi, checkOpenQuestions, checkSkills, checkWorkflows, checkDocs, checkSpecs, checkModuleManifestTemplate, checkCiWorkflows, checkSecurity]
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
