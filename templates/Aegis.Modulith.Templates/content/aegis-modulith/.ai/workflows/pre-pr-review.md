# Workflow: Pre-PR Review

## Purpose

Standard agent workflow for pre-pr review.

## Steps

1. Run npm run check.
2. Run template smoke tests.
3. Review docs/ADR changes.
4. Summarize risks.
5. Ensure git status is clean except intended changes.

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
