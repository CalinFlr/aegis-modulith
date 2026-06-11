---
name: dotnet-architecture-review
description: Use when reviewing module boundaries and architectural consistency.
---

# dotnet-architecture-review

## Goal

Use when reviewing module boundaries and architectural consistency.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Check Domain dependencies.
- Check module references.
- Check DbContext/schema ownership.
- Check no generic repository over EF Core.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
