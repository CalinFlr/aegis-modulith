# Contributing

This repository is intended to be built with strong documentation, guardrails, and template smoke tests.

Before opening a PR:

```bash
npm run check
npm run template:smoke
```

High-risk changes require explicit review:

- Authentication/authorization.
- Migrations.
- Production dependencies.
- CI/release changes.
- Security policy changes.
- License changes.
