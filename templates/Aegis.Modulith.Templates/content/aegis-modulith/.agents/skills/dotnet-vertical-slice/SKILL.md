---
name: dotnet-vertical-slice
description: Use when adding or modifying a CQRS vertical slice in a module.
---

# .NET Vertical Slice

## Goal

Add a command or query slice that stays inside its owning module.

## Procedure

- Commands express business intent.
- Queries do not mutate state.
- Keep request, handler, endpoint, validator, and response files in the feature folder.

## Validation

```bash
dotnet test -c Release
```
