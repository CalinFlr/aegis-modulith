# Plan

## Approach

Implement Aegis.Modulith in layers:

1. Repository and documentation backbone.
2. Node `.mjs` guardrail runner.
3. Template package skeleton.
4. Core profile generated solution.
5. Pro profile additions.
6. MediatR optional path.
7. Advanced profile additions.
8. TaskHub sample.
9. Template smoke matrix and CI.
10. Public polish: README, diagrams, competitive positioning, docs, badges placeholders.

## Files and areas likely affected

- `templates/`
- `src/`
- `tests/`
- `docs/`
- `specs/`
- `.ai/`
- `.agents/skills/`
- `tools/guardrails/`
- `.github/workflows/`

## Validation strategy

- `npm run check`
- `npm run check:ai`
- `npm run check:docs`
- `npm run check:specs`
- `npm run check:security`
- `npm run template:smoke`
- generated solution `dotnet restore`
- generated solution `dotnet build -c Release`
- generated solution `dotnet test -c Release`

## Rollback strategy

Use git milestone commits. If a phase fails, keep prior validated commits and document the blocker in `OpenQuestions.md`.
