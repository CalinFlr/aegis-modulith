---
name: guardrail-runner
description: Use when adding or modifying Node .mjs guardrail checks.
---

# guardrail-runner

## Goal

Use when adding or modifying Node .mjs guardrail checks.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Keep runner cross-platform.
- Use node:child_process with shell false.
- Do not duplicate logic in shell scripts.
- Add tests or sample failing cases.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
