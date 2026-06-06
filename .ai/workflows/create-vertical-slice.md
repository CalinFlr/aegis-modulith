# Workflow: Create Vertical Slice

## Purpose

Standard agent workflow for create vertical slice.

## Steps

1. Read relevant module README.
2. Decide command or query.
3. Create endpoint, command/query, handler, validator if needed, response model.
4. Add tests.
5. Update OpenAPI metadata.
6. Run guardrails.

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
