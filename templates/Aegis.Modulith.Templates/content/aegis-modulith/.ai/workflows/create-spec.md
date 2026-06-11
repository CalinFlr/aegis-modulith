# Workflow: Create Spec

## Purpose

Create a new spec-driven development folder for meaningful work.

## Steps

1. Read `AGENTS.md`.
2. Read root `OpenQuestions.md`.
3. Choose the next numbered folder under `specs/`.
4. Copy the files from `specs/_template/`.
5. Fill in `spec.md` with problem, goals, non-goals, requirements, and acceptance summary.
6. Fill in `plan.md` with implementation approach and validation strategy.
7. Fill in `tasks.md` with atomic tasks.
8. Fill in `risks.md` with technical, security, licensing, and adoption risks.
9. Add local questions to spec `open-questions.md`.
10. Promote blockers or cross-cutting inferred decisions to root `OpenQuestions.md`.

## Required validation

```bash
npm run check:specs
npm run check:docs
```

## Human approval

Required when the spec involves security, licensing, public package behavior, auth/authz, database migration strategy, CI/CD, or public API breaking changes.
