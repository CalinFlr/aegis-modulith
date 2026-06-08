---
name: module-manifest
description: Use when creating or reviewing module.json manifests for generated modules.
---

# Module Manifest

## Goal

Keep module metadata accurate for humans, tools, and AI agents.

## Procedure

- Update name, schema, owner, dependencies, public contracts, features, and rules.
- Keep cross-module database access disabled by default.
- Keep Infrastructure references disabled by default.

## Validation

```bash
dotnet test -c Release
```
