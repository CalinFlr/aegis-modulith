#!/usr/bin/env node

import { existsSync, readFileSync, readdirSync, statSync } from "node:fs";
import { join, relative } from "node:path";

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

async function checkAi() {
  const errors = [];

  if (!existsSync(join(root, "AGENTS.md"))) errors.push("AGENTS.md is missing.");
  if (!existsSync(join(root, "OpenQuestions.md"))) errors.push("OpenQuestions.md is missing.");

  if (existsSync(join(root, ".agents"))) {
    const skills = listFiles(".agents/skills", file => file.endsWith("SKILL.md"));
    if (skills.length === 0) errors.push(".agents/skills contains no SKILL.md files.");
    for (const skill of skills) {
      const content = read(skill);
      if (!content.includes("name:")) errors.push(`${skill} missing name metadata.`);
      if (!content.includes("description:")) errors.push(`${skill} missing description metadata.`);
    }
  }

  if (existsSync(join(root, ".ai"))) {
    const workflows = listFiles(".ai/workflows", file => file.endsWith(".md"));
    const policies = listFiles(".ai/policies", file => file.endsWith(".md") || file.endsWith(".yaml"));
    if (workflows.length === 0) errors.push(".ai/workflows contains no workflow files.");
    if (policies.length === 0) errors.push(".ai/policies contains no policy files.");
  }

  return errors.length ? fail("ai assets", errors) : pass("ai assets");
}

async function checkDocs() {
  const required = [
    "README.md",
    "docs/architecture.md",
    "docs/module-manifest.md"
  ];
  const errors = required
    .filter(file => !existsSync(join(root, file)))
    .map(file => `${file} is missing.`);

  return errors.length ? fail("docs", errors) : pass("docs");
}

async function checkSpecs() {
  const errors = [];
  if (existsSync(join(root, "specs")) && !existsSync(join(root, "specs/README.md"))) {
    errors.push("specs/README.md is missing.");
  }

  return errors.length ? fail("specs", errors) : pass("specs");
}

async function checkModuleManifests() {
  const errors = [];
  const manifests = listFiles("src", file => file.replaceAll("\\", "/").endsWith("/module.json"));

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

async function checkSecurity() {
  const errors = [];

  const suspicious = listFiles(".", file => {
    const normalized = file.replaceAll("\\", "/");
    return normalized.endsWith("/.env") ||
      normalized.includes("/secrets/") ||
      normalized.endsWith("id_rsa");
  });

  for (const file of suspicious) {
    errors.push(`Sensitive-looking file detected: ${file}`);
  }

  const configFiles = listFiles(".", file => {
    const normalized = file.replaceAll("\\", "/").toLowerCase();
    return normalized.endsWith("appsettings.json") ||
      normalized.endsWith("appsettings.production.json");
  });

  for (const file of configFiles) {
    const content = read(file).toLowerCase();
    if (content.includes("password=postgres") || content.includes("\"password\"")) {
      errors.push(`Runtime configuration must not contain a default password: ${file}`);
    }
  }

  return errors.length ? fail("security", errors) : pass("security");
}

const groups = {
  ai: [checkAi],
  docs: [checkDocs],
  specs: [checkSpecs],
  security: [checkSecurity],
  manifest: [checkModuleManifests],
  manifests: [checkModuleManifests],
  all: [checkAi, checkDocs, checkSpecs, checkModuleManifests, checkSecurity]
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
    console.log(`OK ${result.name}`);
  } else {
    failed = true;
    console.error(`FAIL ${result.name}`);
    for (const error of result.errors) console.error(`  - ${error}`);
  }
}

process.exit(failed ? 1 : 0);
