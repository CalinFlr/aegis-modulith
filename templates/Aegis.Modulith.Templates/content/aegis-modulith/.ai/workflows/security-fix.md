# Workflow: Security Fix

## Purpose

Standard agent workflow for security fix.

## Steps

1. Classify risk.
2. Do not expose secrets.
3. Add regression test.
4. Run security checks.
5. Update security docs if needed.

## Required validation

```bash
npm run check
```

## Human approval required when

- Auth/authz changes.
- New production dependency.
- EF migration.
- Public API breaking change.
- CI/release changes.
