---
name: docs-writer
description: Use when updating docs, ADRs, workflows, or README.
---

# docs-writer

## Goal

Use when updating docs, ADRs, workflows, or README.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Explain decisions, not just steps.
- Link docs to guardrails.
- Update indexes.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
