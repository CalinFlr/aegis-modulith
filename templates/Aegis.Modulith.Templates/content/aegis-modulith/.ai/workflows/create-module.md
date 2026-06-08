# Workflow: Create Module

## Purpose

Create or modify a business module without breaking modular boundaries.

## Steps

1. Read `AGENTS.md`.
2. Check `OpenQuestions.md`.
3. Create module files, schema ownership, and `module.json`.
4. Keep Infrastructure private to the module.
5. Add or update tests.

## Required validation

```bash
dotnet build -c Release
dotnet test -c Release
```

## Human approval

Required for public contracts, dependencies, migrations, auth, or security-sensitive behavior.
