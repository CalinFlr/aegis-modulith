---
name: dotnet-module
description: Use when creating or changing a business module.
---

# .NET Module

## Goal

Create or change a business module while preserving ownership boundaries.

## Procedure

- Keep Infrastructure private to the module.
- Keep schema and DbContext ownership inside the module.
- Update `module.json`.
- Add focused tests.

## Validation

```bash
dotnet test -c Release
```
