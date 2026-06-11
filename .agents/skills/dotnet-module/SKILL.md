---
name: dotnet-module
description: Use when creating or changing a business module.
---

# dotnet-module

## Goal

Use when creating or changing a business module.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Create module with Features, Domain, Infrastructure, Contracts.
- Own schema and DbContext.
- Avoid cross-module Infrastructure references.
- Add architecture tests.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
