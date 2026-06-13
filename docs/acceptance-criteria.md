# Acceptance Criteria

## Repository

- [x] Git repository initialized.
- [x] Branch is `main`.
- [x] Apache-2.0 license present.
- [x] README explains value proposition, install, usage, profiles, and examples.
- [x] CONTRIBUTING, SECURITY, CODE_OF_CONDUCT, SUPPORT, and CHANGELOG present or intentionally deferred with TODOs.

## Templates

- [x] `Aegis.Modulith.Templates` builds as a NuGet template package.
- [x] `dotnet new install` works from local package.
- [x] `aegis-modulith` generates a solution.
- [x] `aegis-module` generates a module.
- [x] `aegis-slice` generates command/query slices.
- [x] `aegis-event` generates domain/integration events.
- [x] `aegis-worker` generates worker skeleton.

## Profile validation

- [x] Core profile builds and tests.
- [x] Pro profile builds and tests.
- [x] Advanced profile builds and tests.
- [x] Core mediator builds and tests.
- [x] MediatR option builds and tests.
- [x] TaskHub sample builds and tests.

## Architecture

- [x] Modular monolith structure present.
- [x] Vertical slices inside modules.
- [x] CQRS-lite contracts present.
- [x] PostgreSQL default present.
- [x] Schema-per-module design documented.
- [x] Architecture tests verify module boundaries.
- [x] Event sourcing is not default and is documented as advanced/optional.
- [x] Module manifests are generated for business modules.

## Spec-driven development

- [x] `specs/README.md` present.
- [x] `specs/_template` contains spec, plan, tasks, acceptance, risks, and open-questions templates.
- [x] At least one concrete spec exists.
- [x] `npm run check:specs` validates specs.
- [x] Spec workflows exist for create, review, and implement.

## AI development system

- [x] AGENTS.md present.
- [x] OpenQuestions.md present with required sections.
- [x] CLAUDE.md points to AGENTS.md.
- [x] Copilot instructions point to AGENTS.md.
- [x] `.agents/skills` contains SKILL.md files with metadata.
- [x] `.ai/workflows` contains workflows.
- [x] `.ai/policies` contains risk, dependency, security, and approval policies.
- [x] docs/open-questions-policy.md explains blocker versus inferred decisions.
- [x] Node `.mjs` guardrail runner present.
- [x] No duplicated `.sh`/`.ps1` guardrail logic.
- [x] Module manifest docs and template exist.

## P1A generated AI and guardrail options

- [x] `--ai none` excludes generated AI agent files, `.ai`, `.agents`, AI docs, `OpenQuestions.md`, and `specs`.
- [x] `--ai agents` includes neutral agent files and basic AI docs without enterprise `.ai`, `.agents`, or `specs`.
- [x] `--ai enterprise` includes enterprise `.ai`, generated skills according to `--skills`, AI docs according to `--docs`, and a spec template.
- [x] `--guardrails off` excludes the Node guardrail runner, guardrail package scripts, and guardrail CI wiring.
- [x] `--guardrails standard` includes the generated Node runner, package scripts, generated guardrail checks, and CI wiring.
- [x] `--guardrails strict` includes strict policies/rules and generated strict shape checks.
- [x] `--hooks none` excludes `lefthook.yml`.
- [x] `--hooks lefthook` runs generated guardrails when guardrails are enabled and falls back to dotnet build/test when guardrails are off.
- [x] `--skills none|core|enterprise` materializes the expected generated skill set.
- [x] `--docs standard|full` materializes standard and expanded generated docs distinctly.
- [x] `--license apache2|mit` generates matching `LICENSE`, README license text, and package metadata.
- [x] `npm run template:smoke` asserts the generated semantics above.

## P1B item template semantics

- [x] `aegis-module` generates a module project folder, project file, `IAegisModule` composition entry point, EF Core `DbContext`, service registration extension, module folders, migrations placeholder, and `module.json`.
- [x] `aegis-slice` generates command and query vertical slices under module `Features/<SliceName>` folders with generated CQRS abstractions, endpoint mapping helpers, validators for commands, responses, and paged list-query shape when requested.
- [x] `aegis-event` generates distinct domain and integration events in domain and contracts locations using generated event abstractions.
- [x] `aegis-worker` generates a `BackgroundService` worker with DI registration, logging, and cancellation-token usage.
- [x] `npm run template:smoke` asserts item-template semantics and generated-solution compatibility for core/pro/advanced with core mediator and pro/advanced with MediatR.

## P1C architecture-test expansion

- [x] Generated architecture tests assert module boundary rules, including no cross-module Infrastructure namespace/project references and manifest-declared cross-module Contracts dependencies.
- [x] Generated architecture tests assert module manifest required fields, boundary rule defaults, and declared public contract files.
- [x] Generated architecture tests assert Domain source isolation from ASP.NET Core, EF Core, Npgsql, and Infrastructure usage at the file/namespace level.
- [x] Generated architecture tests assert CQRS request/handler abstractions, MediatR compatibility when selected, query non-mutation, EF no-tracking query convention, and `Features/<SliceName>` slice placement.
- [x] Generated architecture tests assert endpoint mapping conventions, including dispatcher delegation and no direct persistence calls.
- [x] Generated architecture tests assert profile/mediator wiring for core, pro, advanced, core dispatcher, and MediatR variants.
- [x] Generated architecture tests assert module-scoped DbContexts, manifest-aligned schemas, no generated FK configuration by default, and no cross-module domain/infrastructure namespace usage in persistence sources.
- [x] `npm run template:smoke` asserts generated architecture test files, rule coverage markers, manifest-rule assertions, profile/mediator coverage, and absence of unresolved template tokens in architecture tests.
- [x] `docs/architecture/vertical-slices.md` and `docs/architecture/cqrs-lite.md` exist and are linked from the architecture overview.

## P1D-1 pro/advanced feature-depth foundation

- [x] Generated pro and advanced profiles include a Testcontainers PostgreSQL integration test project and package references.
- [x] Generated Testcontainers tests include a PostgreSQL container fixture, API test factory connection-string override, opt-in Docker fact, and database initialization placeholder for future migrations.
- [x] Generated Testcontainers tests are skipped by default unless `AEGIS_RUN_TESTCONTAINERS=true` is set, so default smoke does not require Docker.
- [x] Generated pro and advanced profiles include fake authentication test infrastructure with test users, role/scope headers, authenticated client helpers, and a fake-auth smoke test.
- [x] Fake authentication is isolated to generated test projects and is not wired into production `Program.cs`.
- [x] Generated pro and advanced profiles include Microsoft-first HttpClient resilience defaults through `Microsoft.Extensions.Http.Resilience`, default outbound client registration, and a typed placeholder client.
- [x] Core profile excludes P1D-1 Testcontainers, fake-auth, and HttpClient resilience assets.
- [x] `npm run template:smoke` asserts P1D-1 generated file/package/wiring semantics without requiring Docker.

## P1D-2A auth and permission scaffold

- [x] Generated pro and advanced profiles include JWT bearer package references, typed JWT options, and authentication service registration.
- [x] Generated pro and advanced `Program.cs` wires `UseAuthentication` before `UseAuthorization`.
- [x] Missing JWT issuer, audience, or signing key uses reject-all token validation instead of accepting arbitrary tokens.
- [x] Generated pro and advanced profiles include permission constants, claim-type constants, named policy constants, and policy registration.
- [x] Generated pro and advanced outputs demonstrate named permission policies on protected endpoints without securing every endpoint.
- [x] Generated integration tests include permission-aware fake auth helpers and prove allowed and forbidden permission outcomes.
- [x] Fake auth remains test-only and is not wired into production `Program.cs`.
- [x] Core profile excludes pro/advanced JWT package references, auth middleware, policy registration, and fake auth; it keeps only documented minimal shared permission constants.
- [x] `npm run template:smoke` asserts P1D-2A generated semantics without real JWT issuance or an external identity provider.

## P1D-2B inbox pattern scaffold

- [x] Generated pro and advanced profiles include an inbox persistence model with `MessageId`, `IdempotencyKey`, `MessageType`, serialized payload, processing status, timestamps, failure reason, retry count, and simple lease fields.
- [x] Generated pro and advanced profiles include EF Core inbox configuration, a DbContext, PostgreSQL-friendly `integration.inbox_messages` table naming, and unique indexes for idempotency.
- [x] Generated pro and advanced profiles include an `IInboxStore` abstraction, EF Core implementation, duplicate detection, accepted/duplicate/already-processed results, and processing status updates.
- [x] Generated pro and advanced profiles include an inbox processor scaffold, handler dispatch abstraction, sample integration-event handler, and opt-in hosted processor.
- [x] Generated inbox handlers use module integration event contracts and do not depend on Domain entities or another module's Infrastructure.
- [x] Generated inbox tests prove first acceptance, duplicate detection, processed-message idempotency, failed-message state, and single handler invocation for duplicate inputs without Docker.
- [x] Core profile excludes active inbox infrastructure and generated inbox tests.
- [x] `npm run template:smoke` asserts P1D-2B generated semantics without requiring a broker or Docker.

## P1D-3A generated contract tests

- [x] Generated pro and advanced profiles include `tests/<App>.ContractTests` in the generated solution.
- [x] Generated API contract tests verify OpenAPI production, expected routes and methods, declared content types, declared response status codes, JWT bearer OpenAPI metadata, named permission policy metadata, and fake-auth exclusion from the production API contract.
- [x] Generated API contract tests cover starter endpoints and TaskHub endpoints where selected.
- [x] Generated integration event contract tests verify `System.Text.Json` round trips, integration event type/version metadata, contract-folder placement, and domain/integration event separation.
- [x] Generated inbox contract tests verify payload serialization, `MessageId`, `IdempotencyKey`, message type, and no broker or exactly-once broker guarantee dependency.
- [x] Generated architecture tests assert production projects do not reference contract tests and integration contracts do not depend on infrastructure.
- [x] Core profile excludes the pro/advanced contract test project and documents the exclusion.
- [x] `npm run template:smoke` asserts P1D-3A generated semantics without requiring Docker, a broker, an external identity provider, or external services.

## P1D-3B generated performance smoke tests

- [x] Generated pro and advanced profiles include `tests/<App>.PerformanceSmokeTests` in the generated solution.
- [x] Generated performance smoke tests cover API test host startup, `/health`, authenticated protected request paths through test-only fake auth, generated CQRS request paths, and OpenAPI document generation.
- [x] Generated performance smoke tests use `Stopwatch`, warm-up requests, sample diagnostics, and intentionally loose named thresholds.
- [x] Generated performance smoke tests use EF InMemory and test-local fake auth so default validation does not require Docker, a broker, an external identity provider, a real JWT issuer, a live database, or external services.
- [x] Core profile excludes the pro/advanced performance smoke test project and documents the exclusion.
- [x] Generated architecture tests assert production projects do not reference performance smoke tests.
- [x] `npm run template:smoke` asserts P1D-3B generated semantics without requiring Docker, a broker, an external identity provider, or external services.

## CI and guardrails

- [x] `npm run check` passes.
- [x] `npm run check:ai` passes.
- [x] `npm run check:docs` passes.
- [x] `npm run check:security` passes.
- [x] `npm run check:specs` passes.
- [x] `npm run template:smoke` passes or clearly documents unavailable SDK/package blocker.
- [x] GitHub Actions workflows present.

## Public polish

- [x] `docs/competitive-analysis.md` present.
- [x] `docs/getting-started/which-profile.md` present.
- [x] `docs/public-polish-checklist.md` present.
- [x] README has golden path examples.
- [x] README has profile matrix.
- [x] README has template smoke matrix.

## Final report

- [ ] Codex reports commit list.
- [ ] Codex reports validation commands and outcomes.
- [ ] Codex reports known limitations.
- [ ] Codex reports open blockers and inferred assumptions from OpenQuestions.md.
- [ ] Codex reports specs used or updated.
- [ ] Codex reports next recommended release steps.
