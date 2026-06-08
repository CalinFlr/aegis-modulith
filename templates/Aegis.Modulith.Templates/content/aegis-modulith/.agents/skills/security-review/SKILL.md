---
name: security-review
description: Use when auth, authorization, secrets, dependency, or logging behavior changes.
---

# Security Review

## Goal

Review security-sensitive changes with explicit risk handling.

## Procedure

- Do not read or commit secrets.
- Check auth/authz, logging, dependency, and CI changes.
- Require human review for high or critical risk areas.

## Validation

```bash
npm run check:security
```
