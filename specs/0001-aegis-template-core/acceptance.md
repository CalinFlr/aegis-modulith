# Acceptance Criteria

## Template package

- [ ] `Aegis.Modulith.Templates` builds.
- [ ] Local `dotnet new install` works.
- [ ] `aegis-modulith` generates a solution.
- [ ] Item templates generate modules, slices, events, and workers.

## Smoke matrix

- [ ] `core + core mediator` builds and tests.
- [ ] `core + MediatR` builds and tests.
- [ ] `pro + core mediator` builds and tests.
- [ ] `pro + MediatR` builds and tests.
- [ ] `advanced + core mediator` builds and tests.
- [ ] `advanced + MediatR` builds and tests.
- [ ] `pro + taskhub sample` builds and tests.

## Guardrails

- [ ] `npm run check` passes.
- [ ] `npm run check:specs` passes.
- [ ] `npm run check:manifests` passes.
- [ ] No `.sh` or `.ps1` guardrail logic is added.
- [ ] `OpenQuestions.md` includes blockers or inferred decisions if any remain.

## Documentation

- [ ] README includes golden path.
- [ ] Docs explain profiles, mediator choices, event sourcing decision, database decision, AI development system, and spec-driven workflow.
- [ ] ADRs include spec-driven development and module manifest decisions.
