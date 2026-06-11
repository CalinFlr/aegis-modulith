---
name: module-manifest-review
description: Use when creating or reviewing module.json manifests for generated Aegis.Modulith modules.
---

# Module Manifest Review Skill

## Goal

Ensure module manifests accurately describe module boundaries, schemas, dependencies, public contracts, features, and rules.

## Required context

Read:

- `docs/module-manifest.md`
- `docs/architecture.md`
- `AGENTS.md`

## Rules

- Dependencies must point to contracts, not infrastructure.
- Cross-module database access must remain false by default.
- Cross-module foreign keys must remain false by default.
- Features should map to vertical slices.
- Schema must match the module-owned PostgreSQL schema.

## Validation

```bash
npm run check
npm run check:docs
```
