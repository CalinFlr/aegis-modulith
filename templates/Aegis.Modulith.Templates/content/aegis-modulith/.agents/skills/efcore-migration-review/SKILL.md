---
name: efcore-migration-review
description: Use when adding or reviewing EF Core migrations.
---

# EF Core Migration Review

## Goal

Review migrations for module ownership, schema boundaries, and safe rollout.

## Procedure

- Confirm the migration belongs to one module.
- Avoid cross-module foreign keys.
- Check rollback and data-risk notes.

## Validation

```bash
dotnet test -c Release
```
