# Workflow: Security Fix

## Purpose

Handle security-sensitive changes with explicit review discipline.

## Steps

1. Avoid secrets and local credential files.
2. Document the risk and affected area.
3. Add focused tests.
4. Run security and full checks.

## Required validation

```bash
npm run check:security
npm run check
```

## Human approval

Required before merge.
