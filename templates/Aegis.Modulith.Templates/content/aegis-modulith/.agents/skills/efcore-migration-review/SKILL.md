---
name: efcore-migration-review
description: Use when adding or reviewing EF Core migrations.
---

# efcore-migration-review

## Goal

Use when adding or reviewing EF Core migrations.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Verify owning module.
- Look for destructive changes.
- Add rollback note.
- Run integration tests.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
