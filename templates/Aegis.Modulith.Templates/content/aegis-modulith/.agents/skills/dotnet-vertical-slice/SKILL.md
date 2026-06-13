---
name: dotnet-vertical-slice
description: Use when adding or modifying a CQRS vertical slice in a module.
---

# dotnet-vertical-slice

## Goal

Use when adding or modifying a CQRS vertical slice in a module.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Create endpoint, request, handler, response, validator if needed.
- Keep business logic out of endpoints.
- Add tests and OpenAPI metadata.
- Run npm run check.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
