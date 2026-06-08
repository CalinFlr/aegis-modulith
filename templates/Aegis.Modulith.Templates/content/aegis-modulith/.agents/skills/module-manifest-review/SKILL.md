---
name: module-manifest-review
description: Use when reviewing module.json manifests for consistency and boundary metadata.
---

# Module Manifest Review

## Goal

Review module manifests for accurate ownership and boundary rules.

## Procedure

- Check required fields.
- Check dependencies and public contracts.
- Check boundary rule defaults.

## Validation

```bash
npm run check
```
