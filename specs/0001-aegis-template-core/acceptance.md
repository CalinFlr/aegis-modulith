# Acceptance Criteria

## Template package

- [x] `Aegis.Modulith.Templates` builds.
- [x] Local `dotnet new install` works.
- [x] `aegis-modulith` generates a solution.
- [x] Item templates generate modules, slices, events, and workers.

## Smoke matrix

- [x] `core + core mediator` builds and tests.
- [x] `core + MediatR` builds and tests.
- [x] `pro + core mediator` builds and tests.
- [x] `pro + MediatR` builds and tests.
- [x] `advanced + core mediator` builds and tests.
- [x] `advanced + MediatR` builds and tests.
- [x] `pro + taskhub sample` builds and tests.

## Guardrails

- [x] `npm run check` passes.
- [x] `npm run check:specs` passes.
- [x] `npm run check:manifests` passes.
- [x] No `.sh` or `.ps1` guardrail logic is added.
- [x] `OpenQuestions.md` includes blockers or inferred decisions if any remain.

## Documentation

- [x] README includes golden path.
- [x] Docs explain profiles, mediator choices, event sourcing decision, database decision, AI development system, and spec-driven workflow.
- [x] ADRs include spec-driven development and module manifest decisions.
