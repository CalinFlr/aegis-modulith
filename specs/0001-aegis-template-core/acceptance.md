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

## P1D-1 pro/advanced feature-depth foundation

- [x] Pro and advanced generated outputs include a Testcontainers PostgreSQL integration test project.
- [x] Pro and advanced generated outputs include a PostgreSQL container fixture, API factory connection override, opt-in Docker guard, and migration placeholder.
- [x] Pro and advanced generated outputs include fake authentication test infrastructure, authenticated client helpers, and a fake-auth smoke test.
- [x] Production `Program.cs` does not enable fake authentication.
- [x] Pro and advanced generated outputs include Microsoft HttpClient resilience package references, default registration, and a typed placeholder client.
- [x] Core generated output excludes P1D-1 Testcontainers, fake-auth, and HttpClient resilience assets.
- [x] Default template smoke asserts P1D-1 semantics without requiring Docker.

## P1D-2A auth and permission scaffold

- [x] Pro and advanced generated outputs include JWT bearer package references, typed JWT options, and authentication service registration.
- [x] Pro and advanced generated `Program.cs` calls `UseAuthentication` before `UseAuthorization`.
- [x] Missing JWT configuration rejects tokens by default rather than accepting arbitrary bearer tokens.
- [x] Pro and advanced generated outputs include permission constants, claim-type constants, named policies, and policy registration.
- [x] Generated endpoints demonstrate permission policies for operations and module endpoints, including TaskHub task read/write examples.
- [x] Generated integration tests use test-only fake auth to prove authorized and forbidden permission outcomes without real JWT issuance.
- [x] Core excludes pro/advanced JWT/auth/policy/fake-auth assets except documented minimal shared permission constants.
- [x] Default template smoke asserts P1D-2A semantics without requiring an external identity provider.

## Documentation

- [x] README includes golden path.
- [x] Docs explain profiles, mediator choices, event sourcing decision, database decision, AI development system, and spec-driven workflow.
- [x] ADRs include spec-driven development and module manifest decisions.
