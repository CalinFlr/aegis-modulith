# Workflow: Pre-PR Review

## Purpose

Standard agent workflow for pre-pr review.

## Steps

1. Run `npm run check` when `package.json` exists.
2. When guardrails are disabled and `package.json` is absent, run `dotnet restore`, `dotnet build -c Release`, and `dotnet test -c Release`.
3. Run template smoke tests when the template smoke script exists.
4. Review docs/ADR changes.
5. Summarize risks.
6. Ensure git status is clean except intended changes.

## Required validation

```bash
npm run check
```

If `package.json` is not generated:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Human approval required when

- Auth/authz changes.
- New production dependency.
- EF migration.
- Public API breaking change.
- CI/release changes.
