# Workflow: Implement Spec

## Purpose

Implement one approved spec through small validated steps.

## Steps

1. Read `AGENTS.md`.
2. Read root `OpenQuestions.md`.
3. Read the target spec folder: `spec.md`, `plan.md`, `tasks.md`, `acceptance.md`, `risks.md`, and `open-questions.md`.
4. Confirm no blockers exist.
5. Implement one task or milestone at a time.
6. Run the smallest relevant validation after each milestone.
7. Update `tasks.md` as tasks complete.
8. Update `acceptance.md` when criteria are satisfied.
9. Add or update ADRs when durable architecture decisions are made.
10. Promote cross-cutting questions to root `OpenQuestions.md`.
11. Commit validated milestones.

## Required validation

```bash
npm run check
npm run check:specs
```

When templates exist, also run:

```bash
npm run template:smoke
```

Generated solutions must run:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Human approval

Required before implementing high or critical risk items from `risks.md`.
