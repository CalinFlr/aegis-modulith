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

## P1A generated AI and guardrail options

- [x] Smoke asserts `--ai none|agents|enterprise` generated file shape.
- [x] Smoke asserts `--guardrails off|standard|strict` generated runner, package, CI, and strict artifact shape.
- [x] Smoke asserts `--hooks none|lefthook`, including dotnet-only Lefthook output when guardrails are off.
- [x] Smoke asserts `--skills none|core|enterprise` generated skill sets.
- [x] Smoke asserts `--docs standard|full` generated docs shape.
- [x] Smoke asserts `--license apache2|mit` generated license, README, and package metadata.

## P1B item template semantics

- [x] `aegis-module` output is architecture-integrated and includes an EF Core `DbContext`, `IAegisModule`, service registration, module folders, and `module.json`.
- [x] `aegis-slice` output generates command/query CQRS vertical slices under module feature folders.
- [x] `aegis-event` output distinguishes domain and integration events by location and abstraction.
- [x] `aegis-worker` output uses `BackgroundService`, DI registration, logging, and cancellation tokens.
- [x] Smoke validates item-template outputs against generated core/pro/advanced and MediatR-compatible solutions.

## P1C architecture-test expansion

- [x] Generated architecture tests verify module boundary rules, manifest rules, Domain isolation, CQRS conventions, endpoint mapping discipline, profile/mediator wiring, and module persistence conventions.
- [x] Generated architecture tests run as part of each generated smoke solution.
- [x] Smoke asserts architecture test project/files, coverage markers, module manifest assertions, profile/mediator assertions, and no unresolved architecture-test template tokens.
- [x] Architecture documentation includes focused CQRS-lite and vertical-slice rule pages.

## Documentation

- [x] README includes golden path.
- [x] Docs explain profiles, mediator choices, event sourcing decision, database decision, AI development system, and spec-driven workflow.
- [x] ADRs include spec-driven development and module manifest decisions.
