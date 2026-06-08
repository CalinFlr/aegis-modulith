# Workflow: Create Vertical Slice

## Purpose

Add a command or query slice inside one module.

## Steps

1. Read the module manifest.
2. Put request, handler, endpoint, validator, and response types in the feature folder.
3. Keep commands business-intent based.
4. Keep queries read-only.
5. Add focused tests.

## Required validation

```bash
dotnet test -c Release
```

## Human approval

Required for public API contract changes before merge.
