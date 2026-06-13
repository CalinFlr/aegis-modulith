---
name: spec-driven-feature
description: Use when creating or implementing a spec-driven feature in Aegis.Modulith. Applies to specs, plans, tasks, acceptance criteria, risks, and OpenQuestions handling.
---

# Spec Driven Feature Skill

## Goal

Turn a feature idea into a durable spec, plan, task list, acceptance criteria, and validated implementation path.

## Required context

Read:

- `AGENTS.md`
- `OpenQuestions.md`
- `specs/README.md`
- `.ai/workflows/create-spec.md`
- `.ai/workflows/implement-spec.md`

## Rules

- Do not implement large ambiguous work without a spec.
- Keep specs focused on one feature or capability.
- Use `OpenQuestions.md` for blockers and project-wide inferred decisions.
- Use ADRs for durable architecture decisions.
- Keep tasks atomic and commit-friendly.

## Validation

Run:

```bash
npm run check:specs
npm run check:docs
```
