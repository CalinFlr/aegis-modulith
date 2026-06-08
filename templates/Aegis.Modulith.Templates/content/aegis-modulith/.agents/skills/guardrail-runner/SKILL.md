---
name: guardrail-runner
description: Use when adding or modifying Node .mjs guardrail checks.
---

# Guardrail Runner

## Goal

Keep guardrail checks deterministic, cross-platform, and implemented in Node `.mjs`.

## Procedure

- Use `node:child_process` with `shell: false`.
- Do not duplicate check logic in shell scripts.
- Add semantic checks for generated behavior.

## Validation

```bash
npm run check
```
