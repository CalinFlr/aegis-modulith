---
name: security-review
description: Use when auth, authorization, secrets, dependency, or logging behavior changes.
---

# security-review

## Goal

Use when auth, authorization, secrets, dependency, or logging behavior changes.

## Required context

Read `AGENTS.md` first. Then read the relevant file under `.ai/workflows` and `.ai/policies`.

## Procedure

- Classify risk.
- Check no secret leaks.
- Check no PII logging.
- Require human approval for high-risk changes.

## Validation

Run the smallest relevant checks, and before completion run:

```bash
npm run check
```

## Final response

Report files changed, validation commands, risks, and remaining limitations.
