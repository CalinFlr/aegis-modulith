#!/usr/bin/env node

import { existsSync, mkdirSync, readFileSync, readdirSync, statSync, writeFileSync } from "node:fs";
import { join, relative } from "node:path";
import { spawn } from "node:child_process";
import { randomUUID } from "node:crypto";

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

function readAbsolute(path) {
  return readFileSync(path, "utf8");
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

function assertExists(errors, path, message) {
  if (!existsSync(path)) {
    errors.push(message);
  }
}

function assertMissing(errors, path, message) {
  if (existsSync(path)) {
    errors.push(message);
  }
}

function assertContains(errors, path, expected, message) {
  if (!existsSync(path)) {
    errors.push(`${message} Missing file: ${path}.`);
    return;
  }

  const content = readAbsolute(path);
  if (!content.includes(expected)) {
    errors.push(message);
  }
}

function assertNotContains(errors, path, unexpected, message) {
  if (!existsSync(path)) {
    return;
  }

  const content = readAbsolute(path);
  if (content.includes(unexpected)) {
    errors.push(message);
  }
}

function assertGeneratedOptions(errors, output, variant) {
  const props = join(output, "Directory.Build.props");
  assertContains(errors, props, `<AegisProfile>${variant.profile}</AegisProfile>`, `${variant.id} should record the selected profile.`);
  assertContains(errors, props, `<AegisMediator>${variant.mediator}</AegisMediator>`, `${variant.id} should record the selected mediator.`);
  assertContains(errors, props, `<AegisSample>${variant.sample}</AegisSample>`, `${variant.id} should record the selected sample.`);
  assertContains(errors, props, `<AegisAi>${variant.ai}</AegisAi>`, `${variant.id} should record the selected AI mode.`);
  assertContains(errors, props, `<AegisGuardrails>${variant.guardrails}</AegisGuardrails>`, `${variant.id} should record the selected guardrails mode.`);
  assertContains(errors, props, `<AegisHooks>${variant.hooks}</AegisHooks>`, `${variant.id} should record the selected hooks mode.`);
  assertContains(errors, props, `<AegisSkills>${variant.skills}</AegisSkills>`, `${variant.id} should record the selected skills mode.`);
  assertContains(errors, props, `<AegisDocs>${variant.docs}</AegisDocs>`, `${variant.id} should record the selected docs mode.`);
  assertContains(errors, props, `<AegisLicense>${variant.licenseExpression}</AegisLicense>`, `${variant.id} should record the selected license.`);
}

function assertMediatorSemantics(errors, output, variant) {
  const dispatching = join(output, "src", `${variant.name}.BuildingBlocks`, "Cqrs", "DispatchingServiceCollectionExtensions.cs");
  const module = variant.sample === "taskhub" ? "Projects" : "WorkItems";
  const feature = variant.sample === "taskhub" ? "CreateProject" : "CreateWorkItem";
  const command = join(output, "src", `${variant.name}.Modules`, "Modules", module, "Features", feature, `${feature}Command.cs`);
  const handler = join(output, "src", `${variant.name}.Modules`, "Modules", module, "Features", feature, `${feature}Handler.cs`);

  if (variant.mediator === "mediatr") {
    assertContains(errors, dispatching, "services.AddMediatR", `${variant.id} should register MediatR services.`);
    assertContains(errors, dispatching, "MediatRCommandDispatcher", `${variant.id} should use the MediatR command dispatcher.`);
    assertContains(errors, dispatching, "MediatRQueryDispatcher", `${variant.id} should use the MediatR query dispatcher.`);
    assertContains(errors, dispatching, "ISender sender", `${variant.id} should dispatch through MediatR ISender.`);
    assertNotContains(errors, dispatching, "RegisterCoreHandlers(services", `${variant.id} should not register core handlers in MediatR mode.`);
    assertNotContains(errors, dispatching, "ServiceProviderCommandDispatcher", `${variant.id} should not use the core command dispatcher in MediatR mode.`);
    assertContains(errors, command, "MediatR.IRequest<", `${variant.id} commands should implement MediatR.IRequest.`);
    assertContains(errors, handler, "MediatR.IRequestHandler<", `${variant.id} handlers should implement MediatR.IRequestHandler.`);
    return;
  }

  assertContains(errors, dispatching, "RegisterCoreHandlers(services", `${variant.id} should register core CQRS handlers.`);
  assertContains(errors, dispatching, "ServiceProviderCommandDispatcher", `${variant.id} should use the core command dispatcher.`);
  assertContains(errors, dispatching, "ServiceProviderQueryDispatcher", `${variant.id} should use the core query dispatcher.`);
  assertNotContains(errors, dispatching, "services.AddMediatR", `${variant.id} should not register MediatR services in core mediator mode.`);
  assertNotContains(errors, dispatching, "MediatRCommandDispatcher", `${variant.id} should not use the MediatR dispatcher in core mediator mode.`);
  assertNotContains(errors, command, "MediatR.IRequest<", `${variant.id} commands should not implement MediatR.IRequest in core mediator mode.`);
  assertNotContains(errors, handler, "MediatR.IRequestHandler<", `${variant.id} handlers should not implement MediatR.IRequestHandler in core mediator mode.`);
}

function assertProfileSemantics(errors, output, variant) {
  const solution = join(output, `${variant.name}.sln`);
  const program = join(output, "src", `${variant.name}.Api`, "Program.cs");
  const apiProject = join(output, "src", `${variant.name}.Api`, `${variant.name}.Api.csproj`);
  const appHostProject = join(output, "src", `${variant.name}.AppHost`, `${variant.name}.AppHost.csproj`);
  const serviceDefaultsProject = join(output, "src", `${variant.name}.ServiceDefaults`, `${variant.name}.ServiceDefaults.csproj`);
  const dockerfile = join(output, "Dockerfile");
  const proServices = join(output, "src", `${variant.name}.Api`, "Pro", "ProProfileServices.cs");
  const advancedServices = join(output, "src", `${variant.name}.Api`, "Advanced", "AdvancedProfileServices.cs");

  if (variant.profile === "core") {
    assertMissing(errors, appHostProject, `${variant.id} core profile should not include AppHost.`);
    assertMissing(errors, serviceDefaultsProject, `${variant.id} core profile should not include ServiceDefaults.`);
    assertMissing(errors, dockerfile, `${variant.id} core profile should not include Dockerfile.`);
    assertMissing(errors, proServices, `${variant.id} core profile should not include pro profile services.`);
    assertMissing(errors, advancedServices, `${variant.id} core profile should not include advanced profile services.`);
    assertNotContains(errors, apiProject, "ServiceDefaults", `${variant.id} API project should not reference ServiceDefaults.`);
    assertNotContains(errors, solution, "AppHost", `${variant.id} solution should not reference AppHost.`);
    assertNotContains(errors, solution, "ServiceDefaults", `${variant.id} solution should not reference ServiceDefaults.`);
    assertNotContains(errors, program, "AddProProfileServices", `${variant.id} Program.cs should not wire pro services.`);
    assertNotContains(errors, program, "MapProProfileEndpoints", `${variant.id} Program.cs should not map pro endpoints.`);
    assertNotContains(errors, program, "AddAdvancedProfileServices", `${variant.id} Program.cs should not wire advanced services.`);
    assertNotContains(errors, program, "MapAdvancedProfileEndpoints", `${variant.id} Program.cs should not map advanced endpoints.`);
    return;
  }

  assertExists(errors, appHostProject, `${variant.id} should include AppHost.`);
  assertExists(errors, serviceDefaultsProject, `${variant.id} should include ServiceDefaults.`);
  assertExists(errors, dockerfile, `${variant.id} should include Dockerfile.`);
  assertExists(errors, proServices, `${variant.id} should include pro profile services.`);
  assertContains(errors, apiProject, "ServiceDefaults", `${variant.id} API project should reference ServiceDefaults.`);
  assertContains(errors, solution, "AppHost", `${variant.id} solution should reference AppHost.`);
  assertContains(errors, solution, "ServiceDefaults", `${variant.id} solution should reference ServiceDefaults.`);
  assertContains(errors, program, "builder.AddServiceDefaults();", `${variant.id} Program.cs should wire ServiceDefaults.`);
  assertContains(errors, program, "builder.Services.AddProProfileServices();", `${variant.id} Program.cs should register pro services.`);
  assertContains(errors, program, "app.UseRateLimiter();", `${variant.id} Program.cs should enable rate limiting middleware.`);
  assertContains(errors, program, "app.MapProProfileEndpoints();", `${variant.id} Program.cs should map pro endpoints.`);

  if (variant.profile === "advanced") {
    assertExists(errors, advancedServices, `${variant.id} should include advanced profile services.`);
    assertContains(errors, program, "builder.Services.AddAdvancedProfileServices();", `${variant.id} Program.cs should register advanced services.`);
    assertContains(errors, program, "app.MapAdvancedProfileEndpoints();", `${variant.id} Program.cs should map advanced endpoints.`);
  } else {
    assertMissing(errors, advancedServices, `${variant.id} pro profile should not include advanced profile services.`);
    assertNotContains(errors, program, "AddAdvancedProfileServices", `${variant.id} pro Program.cs should not wire advanced services.`);
    assertNotContains(errors, program, "MapAdvancedProfileEndpoints", `${variant.id} pro Program.cs should not map advanced endpoints.`);
  }
}

function assertAiSemantics(errors, output, variant) {
  const aiFiles = [
    "AGENTS.md",
    "CLAUDE.md",
    ".github/copilot-instructions.md",
    "OpenQuestions.md",
    ".ai",
    ".agents",
    "docs/ai-development",
    "specs"
  ];

  if (variant.ai === "none") {
    for (const file of aiFiles) {
      assertMissing(errors, join(output, file), `${variant.id} ai=none should not include ${file}.`);
    }
    return;
  }

  assertExists(errors, join(output, "AGENTS.md"), `${variant.id} should include AGENTS.md.`);
  assertExists(errors, join(output, "CLAUDE.md"), `${variant.id} should include CLAUDE.md.`);
  assertContains(errors, join(output, "CLAUDE.md"), "AGENTS.md", `${variant.id} CLAUDE.md should point to AGENTS.md.`);
  assertExists(errors, join(output, ".github", "copilot-instructions.md"), `${variant.id} should include Copilot instructions because GitHub workflow output exists.`);
  assertContains(errors, join(output, ".github", "copilot-instructions.md"), "AGENTS.md", `${variant.id} Copilot instructions should point to AGENTS.md.`);
  assertExists(errors, join(output, "OpenQuestions.md"), `${variant.id} should include OpenQuestions.md.`);

  if (variant.ai === "agents") {
    for (const file of [".ai", ".agents", "specs"]) {
      assertMissing(errors, join(output, file), `${variant.id} ai=agents should not include enterprise-only ${file}.`);
    }
    return;
  }

  for (const file of [".ai/policies", ".ai/workflows", ".ai/guardrails", ".ai/evals"]) {
    assertExists(errors, join(output, file), `${variant.id} ai=enterprise should include ${file}.`);
  }

  assertExists(errors, join(output, "specs", "README.md"), `${variant.id} ai=enterprise should include specs/README.md.`);
  for (const file of ["spec.md", "plan.md", "tasks.md", "acceptance.md", "risks.md", "open-questions.md"]) {
    assertExists(errors, join(output, "specs", "_template", file), `${variant.id} ai=enterprise should include specs/_template/${file}.`);
  }
}

function assertGuardrailSemantics(errors, output, variant) {
  const runner = join(output, "tools", "guardrails", "check.mjs");
  const packageJson = join(output, "package.json");
  const ci = join(output, ".github", "workflows", "ci.yml");

  if (variant.guardrails === "off") {
    assertMissing(errors, runner, `${variant.id} guardrails=off should not include the Node guardrail runner.`);
    assertMissing(errors, packageJson, `${variant.id} guardrails=off should not include guardrail package scripts.`);
    assertNotContains(errors, ci, "npm run check", `${variant.id} guardrails=off CI should not run guardrails.`);
  } else {
    assertExists(errors, runner, `${variant.id} guardrails=${variant.guardrails} should include the Node guardrail runner.`);
    assertExists(errors, packageJson, `${variant.id} guardrails=${variant.guardrails} should include package.json.`);
    for (const script of ["\"check\"", "\"check:ai\"", "\"check:docs\"", "\"check:security\"", "\"check:specs\"", "\"template:smoke\""]) {
      assertContains(errors, packageJson, script, `${variant.id} package.json should include ${script}.`);
    }
    assertContains(errors, ci, "npm run check", `${variant.id} CI should run generated guardrails.`);
  }

  if (variant.guardrails === "strict" && variant.ai === "enterprise") {
    assertExists(errors, join(output, ".ai", "policies", "strict-mode.md"), `${variant.id} strict enterprise output should include strict policy.`);
    assertExists(errors, join(output, ".ai", "guardrails", "strict-rules.md"), `${variant.id} strict enterprise output should include strict rules.`);
  } else {
    assertMissing(errors, join(output, ".ai", "policies", "strict-mode.md"), `${variant.id} should not include strict policy outside strict enterprise output.`);
    assertMissing(errors, join(output, ".ai", "guardrails", "strict-rules.md"), `${variant.id} should not include strict rules outside strict enterprise output.`);
  }
}

function assertHookSemantics(errors, output, variant) {
  const lefthook = join(output, "lefthook.yml");

  if (variant.hooks !== "lefthook") {
    assertMissing(errors, lefthook, `${variant.id} hooks=none variant should not include lefthook.yml.`);
    return;
  }

  assertExists(errors, lefthook, `${variant.id} hooks=lefthook variant should include lefthook.yml.`);
  if (variant.guardrails === "off") {
    assertContains(errors, lefthook, "dotnet build -c Release", `${variant.id} guardrails=off lefthook should run dotnet build.`);
    assertContains(errors, lefthook, "dotnet test -c Release --no-build", `${variant.id} guardrails=off lefthook should run dotnet test.`);
    assertNotContains(errors, lefthook, "npm run check", `${variant.id} guardrails=off lefthook should not call npm guardrails.`);
  } else {
    assertContains(errors, lefthook, "npm run check", `${variant.id} guardrails=${variant.guardrails} lefthook should run npm guardrails.`);
  }
}

function assertSkillSemantics(errors, output, variant) {
  const coreSkills = [
    "docs-writer",
    "dotnet-architecture-review",
    "dotnet-module",
    "dotnet-vertical-slice",
    "module-manifest",
    "spec-driven-feature"
  ];
  const enterpriseOnly = [
    "competitive-review",
    "efcore-migration-review",
    "guardrail-runner",
    "module-manifest-review",
    "openapi-contract-review",
    "security-review"
  ];
  const enterpriseSkills = [...coreSkills, ...enterpriseOnly];

  if (variant.ai !== "enterprise" || variant.skills === "none") {
    assertMissing(errors, join(output, ".agents", "skills"), `${variant.id} should not include .agents/skills.`);
    return;
  }

  const expected = variant.skills === "core" ? coreSkills : enterpriseSkills;
  for (const skill of expected) {
    assertExists(errors, join(output, ".agents", "skills", skill, "SKILL.md"), `${variant.id} skills=${variant.skills} missing ${skill}.`);
  }

  if (variant.skills === "core") {
    for (const skill of enterpriseOnly) {
      assertMissing(errors, join(output, ".agents", "skills", skill), `${variant.id} skills=core should not include enterprise-only ${skill}.`);
    }
  }
}

function assertDocsSemantics(errors, output, variant) {
  for (const file of ["docs/getting-started.md", "docs/architecture.md", "docs/development.md", "docs/module-manifest.md"]) {
    assertExists(errors, join(output, file), `${variant.id} should include standard doc ${file}.`);
  }

  if (variant.docs === "standard") {
    for (const file of ["docs/security.md", "docs/operations.md", "docs/adr", "docs/ai-development"]) {
      assertMissing(errors, join(output, file), `${variant.id} docs=standard should not include expanded ${file}.`);
    }
    return;
  }

  for (const file of ["docs/security.md", "docs/operations.md", "docs/adr/0001-modular-monolith.md"]) {
    assertExists(errors, join(output, file), `${variant.id} docs=full should include ${file}.`);
  }

  if (variant.ai === "none") {
    assertMissing(errors, join(output, "docs", "ai-development"), `${variant.id} ai=none should not include AI-specific docs.`);
  } else if (variant.ai === "agents") {
    for (const file of ["agent-operating-model.md", "ai-pr-protocol.md"]) {
      assertExists(errors, join(output, "docs", "ai-development", file), `${variant.id} ai=agents docs=full should include basic AI doc ${file}.`);
    }
    for (const file of ["guardrails.md", "skills.md", "spec-driven-development.md", "workflows.md"]) {
      assertMissing(errors, join(output, "docs", "ai-development", file), `${variant.id} ai=agents should not include enterprise AI doc ${file}.`);
    }
  } else {
    for (const file of ["agent-operating-model.md", "ai-pr-protocol.md", "guardrails.md", "skills.md", "spec-driven-development.md", "workflows.md"]) {
      assertExists(errors, join(output, "docs", "ai-development", file), `${variant.id} ai=enterprise docs=full should include ${file}.`);
    }
  }
}

function assertLicenseSemantics(errors, output, variant) {
  const license = join(output, "LICENSE");
  const readme = join(output, "README.md");
  const packageJson = join(output, "package.json");

  assertExists(errors, license, `${variant.id} should include LICENSE.`);
  assertContains(errors, readme, `License: \`${variant.licenseExpression}\``, `${variant.id} README should report ${variant.licenseExpression}.`);

  if (variant.license === "apache2") {
    assertContains(errors, license, "Apache License", `${variant.id} apache2 license should contain Apache License.`);
    assertContains(errors, license, "Version 2.0", `${variant.id} apache2 license should contain Version 2.0.`);
    assertNotContains(errors, license, "MIT License", `${variant.id} apache2 license should not contain MIT License.`);
  } else {
    assertContains(errors, license, "MIT License", `${variant.id} mit license should contain MIT License.`);
    assertContains(errors, license, "Permission is hereby granted", `${variant.id} mit license should contain MIT grant text.`);
    assertNotContains(errors, license, "Apache License", `${variant.id} mit license should not contain Apache License.`);
  }

  if (existsSync(packageJson)) {
    assertContains(errors, packageJson, `"license": "${variant.licenseExpression}"`, `${variant.id} package metadata should report ${variant.licenseExpression}.`);
  }
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

  const smokeRootBase = join(root, "artifacts", "template-smoke");
  const runsRoot = join(smokeRootBase, "runs");
  const runId = `${Date.now().toString(36)}-${randomUUID().slice(0, 8)}`;
  const smokeRoot = join(runsRoot, runId);

  mkdirSync(smokeRoot, { recursive: true });
  try {
    writeFileSync(join(smokeRootBase, "latest-run.txt"), `${smokeRoot}\n`, "utf8");
  } catch (error) {
    console.warn(`Could not update latest smoke run pointer: ${error.message}`);
  }
  console.log(`Template smoke run directory: ${smokeRoot}`);

  const packagesDir = join(smokeRoot, "p");
  const generatedRoot = join(smokeRoot, "g");
  const itemRoot = join(smokeRoot, "i");
  const dotnetHome = join(smokeRoot, "h");
  const nugetPackages = join(smokeRoot, "n");

  mkdirSync(packagesDir, { recursive: true });
  mkdirSync(generatedRoot, { recursive: true });
  mkdirSync(itemRoot, { recursive: true });
  mkdirSync(dotnetHome, { recursive: true });
  mkdirSync(nugetPackages, { recursive: true });

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
    DOTNET_NOLOGO: "1",
    NUGET_PACKAGES: nugetPackages
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
    { id: "core-core", name: "Smoke.CoreCore", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--mediator", "core"] },
    { id: "core-mediatr", name: "Smoke.CoreMediatR", profile: "core", mediator: "mediatr", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--mediator", "mediatr"] },
    { id: "pro-core", name: "Smoke.ProCore", profile: "pro", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "pro", "--mediator", "core"] },
    { id: "pro-mediatr", name: "Smoke.ProMediatR", profile: "pro", mediator: "mediatr", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "pro", "--mediator", "mediatr"] },
    { id: "advanced-core", name: "Smoke.AdvancedCore", profile: "advanced", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "advanced", "--mediator", "core"] },
    { id: "advanced-mediatr", name: "Smoke.AdvancedMediatR", profile: "advanced", mediator: "mediatr", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "advanced", "--mediator", "mediatr"] },
    { id: "taskhub", name: "Aegis.TaskHub", profile: "pro", mediator: "core", sample: "taskhub", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "pro", "--sample", "taskhub"] },
    { id: "strict-enterprise", name: "Smoke.StrictEnterprise", profile: "advanced", mediator: "core", sample: "none", ai: "enterprise", guardrails: "strict", hooks: "lefthook", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "advanced", "--ai", "enterprise", "--guardrails", "strict", "--hooks", "lefthook"] },
    { id: "ai-none", name: "Smoke.AiNone", profile: "core", mediator: "core", sample: "none", ai: "none", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "none", "--guardrails", "standard", "--docs", "full"] },
    { id: "ai-agents", name: "Smoke.AiAgents", profile: "core", mediator: "core", sample: "none", ai: "agents", guardrails: "standard", hooks: "none", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "agents", "--guardrails", "standard", "--docs", "full"] },
    { id: "guardrails-off-lefthook", name: "Smoke.GuardrailsOff", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "off", hooks: "lefthook", skills: "enterprise", docs: "full", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "enterprise", "--guardrails", "off", "--hooks", "lefthook"] },
    { id: "skills-none-docs-standard", name: "Smoke.SkillsNoneDocsStandard", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "none", docs: "standard", license: "apache2", licenseExpression: "Apache-2.0", args: ["--profile", "core", "--ai", "enterprise", "--skills", "none", "--docs", "standard", "--guardrails", "standard"] },
    { id: "skills-core-license-mit", name: "Smoke.SkillsCoreLicenseMit", profile: "core", mediator: "core", sample: "none", ai: "enterprise", guardrails: "standard", hooks: "none", skills: "core", docs: "full", license: "mit", licenseExpression: "MIT", args: ["--profile", "core", "--ai", "enterprise", "--skills", "core", "--license", "mit", "--guardrails", "standard"] }
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
      output
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

    assertGeneratedOptions(errors, output, variant);
    assertMediatorSemantics(errors, output, variant);
    assertProfileSemantics(errors, output, variant);
    assertAiSemantics(errors, output, variant);
    assertGuardrailSemantics(errors, output, variant);
    assertHookSemantics(errors, output, variant);
    assertSkillSemantics(errors, output, variant);
    assertDocsSemantics(errors, output, variant);
    assertLicenseSemantics(errors, output, variant);

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
    }

    if (variant.guardrails !== "off") {
      code = await runCommand(process.execPath, ["tools/guardrails/check.mjs", "all"], { cwd: output, env: smokeEnv });
      if (code !== 0) {
        return fail("template smoke", [`Generated guardrails failed for ${variant.id}.`]);
      }
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
    code = await runCommand("dotnet", item.args, { env: smokeEnv });
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
