# Workflow: Add Outbox Flow

## Purpose

Standard agent workflow for add outbox flow.

## Steps

1. Create domain event.
2. Map to integration event.
3. Persist outbox message in same transaction.
4. Add dispatcher/worker behavior.
5. Add idempotency tests.

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
