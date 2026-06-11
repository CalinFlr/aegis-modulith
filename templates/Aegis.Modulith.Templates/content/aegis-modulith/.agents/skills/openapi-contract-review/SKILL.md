---
name: openapi-contract-review
description: Use when public API endpoints or contracts change.
---

# openapi-contract-review

## Goal

Use when public API endpoints or contracts change.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Check ProblemDetails.
- Check response DTOs.
- Check pagination for collections.
- Update docs.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
