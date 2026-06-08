---
name: openapi-contract-review
description: Use when public API endpoints or contracts change.
---

# OpenAPI Contract Review

## Goal

Review public endpoint and DTO changes before merge.

## Procedure

- Check request and response DTOs.
- Avoid exposing EF entities.
- Document breaking changes.

## Validation

```bash
dotnet test -c Release
```
