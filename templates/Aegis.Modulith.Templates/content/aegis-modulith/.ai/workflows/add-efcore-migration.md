# Workflow: Add EF Core Migration

## Purpose

Standard agent workflow for add ef core migration.

## Steps

1. Identify owning module DbContext.
2. Add migration to module only.
3. Review generated migration for destructive changes.
4. Document rollback note.
5. Run integration tests.

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
