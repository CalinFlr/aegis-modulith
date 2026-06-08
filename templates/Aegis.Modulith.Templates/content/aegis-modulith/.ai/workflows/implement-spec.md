# Workflow: Implement Spec

## Purpose

Implement a spec through small validated changes.

## Steps

1. Read the target spec folder.
2. Confirm no blockers exist.
3. Implement one task at a time.
4. Update tasks and acceptance when proven.
5. Run relevant checks.

## Required validation

```bash
npm run check
dotnet test -c Release
```

## Human approval

Required for high or critical risk changes.
