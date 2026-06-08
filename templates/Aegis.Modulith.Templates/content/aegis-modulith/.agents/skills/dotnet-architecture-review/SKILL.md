---
name: dotnet-architecture-review
description: Use when reviewing module boundaries and architectural consistency.
---

# .NET Architecture Review

## Goal

Review generated modular monolith code for boundary, CQRS, and persistence consistency.

## Procedure

- Check module boundaries.
- Check commands and queries remain separate.
- Check API responses do not expose EF entities.
- Check no generic EF repository abstraction is introduced.

## Validation

```bash
dotnet test -c Release
```
