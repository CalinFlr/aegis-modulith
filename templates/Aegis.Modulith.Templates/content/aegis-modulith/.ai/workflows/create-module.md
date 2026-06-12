# Workflow: Create Module

## Purpose

Standard agent workflow for create module.

## Steps

1. Read AGENTS.md and architecture docs.
2. Choose module name and schema.
3. Create module project, Contracts, Domain, Infrastructure, Features folders.
4. Add module registration.
5. Add architecture tests.
6. Update docs.

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
