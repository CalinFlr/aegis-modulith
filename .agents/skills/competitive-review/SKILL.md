---
name: competitive-review
description: Use when making product, documentation, or positioning decisions by comparing Aegis.Modulith with other .NET starter, modular monolith, or AI-assisted development repositories.
---

# Competitive Review Skill

## Goal

Ensure Aegis.Modulith learns from strong repositories without copying their weaknesses or drifting from its core positioning.

## Required context

Read:

- `docs/competitive-analysis.md`
- `docs/project-brief.md`
- `docs/architecture.md`
- `docs/getting-started/which-profile.md`
- `OpenQuestions.md`

## Rules

- Do not turn Aegis into a heavy framework.
- Do not make UI default.
- Do not make microservices default.
- Do not make event sourcing default.
- Do not make MediatR default.
- Preserve Aegis positioning as an AI-ready .NET modular monolith template system.

## Validation

Run:

```bash
npm run check:docs
npm run check:specs
```
