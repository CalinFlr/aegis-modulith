---
name: module-manifest
description: Use when creating or reviewing module.json manifests for generated Aegis.Modulith modules.
---

# Module Manifest Skill

## Goal

Make module ownership, dependencies, public contracts, features, and boundary rules explicit.

## Required context

Read:

- `docs/module-manifest.md`
- `docs/architecture.md`
- `AGENTS.md`
- the module's `module.json`, if it exists

## Rules

- Every generated business module should include `module.json`.
- The manifest must list name, schema, type, owner, dependencies, publicContracts, features, and rules.
- Dependencies must be explicit.
- The manifest does not replace architecture tests.
- Do not use the manifest to justify boundary violations.

## Validation

```bash
npm run check:specs
npm run check:ai
```
