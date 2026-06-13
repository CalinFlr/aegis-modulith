# P1D-3A Contract Tests Verification

Date: 2026-06-09

## Summary

P1D-3A is implemented and verified.

This work added generated API and integration-contract test foundations for `pro` and `advanced` outputs. It did not start P1D-3B performance smoke tests, P1D-4 deployment skeleton work, UI, event sourcing, broker integration, public screenshots, badges, docs-site work, P2 public polish, or project renaming.

Fresh validation evidence:

- First successful smoke run: `artifacts/template-smoke/runs/mq6y1hqg-4c09fc52`
- Immediate second successful smoke run: `artifacts/template-smoke/runs/mq6yhsor-2804b561`
- Primary generated evidence source: `artifacts/template-smoke/runs/mq6yhsor-2804b561`

## P1D-3A Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced contract test project | Pass | Generated `pro` and `advanced` outputs include `tests/<App>.ContractTests` and the project is included in the generated solution. |
| Core exclusion | Pass | Generated core outputs do not include `tests/<App>.ContractTests`; generated `docs/contracts.md` explains the exclusion. |
| Mediator compatibility | Pass | `pro`/`advanced` with both `mediator=core` and `mediator=mediatr` built and ran generated contract tests. |
| API contract tests | Pass | Generated `ApiContractTests` verify OpenAPI production, route/method/status/content-type semantics, bearer security scheme metadata, named authorization policy metadata, and fake-auth exclusion. |
| OpenAPI JWT bearer scheme | Pass | Generated pro/advanced APIs register OpenAPI bearer metadata through `AddAegisOpenApi` and `BearerSecuritySchemeTransformer`. |
| Protected endpoint metadata | Pass | Contract tests assert protected generated endpoints use named `AegisAuthorizationPolicies` constants. |
| Fake auth isolation | Pass | Contract and architecture tests assert production API source and OpenAPI output do not expose `Aegis.Test`, fake-auth types, or `X-Test-*` headers. |
| Integration event metadata | Pass | Generated integration event records declare `IntegrationEventContractAttribute` type/version metadata. |
| Integration event round trips | Pass | Generated `IntegrationEventContractTests` serialize and deserialize integration events with `System.Text.Json` and compare public properties. |
| Domain/integration separation | Pass | Generated tests assert domain events are not integration events and integration events live under Contracts namespaces. |
| Inbox contract checks | Pass | Generated `InboxContractTests` verify `MessageId`, `IdempotencyKey`, message type, and serialized payload round-trip behavior. |
| No broker dependency | Pass | Generated contract tests and smoke assertions check for no generated broker dependency markers; no broker is required for default validation. |
| No event sourcing | Pass | Event metadata is documented as contract-test metadata only; no event store or event-sourcing implementation was introduced. |
| Architecture boundaries | Pass | Generated `ContractBoundaryTests` assert contract tests are not referenced by production projects and integration contracts do not depend on infrastructure. |
| Smoke semantic assertions | Pass | Root `template:smoke` now asserts P1D-3A project shape, test markers, OpenAPI/auth-policy coverage, integration event coverage, inbox contract coverage, core exclusion, and no unresolved generated contract-test tokens. |
| No external runtime requirement | Pass | Default validation used generated `dotnet test`; Docker-backed Testcontainers tests remained skipped by default and contract tests required no Docker, broker, external identity provider, real JWT issuer, or external services. |

## Generated Output Evidence

Representative pro output from `mq6yhsor-2804b561`:

- `g/pro-core/tests/Smoke.ProCore.ContractTests/Smoke.ProCore.ContractTests.csproj`
- `g/pro-core/tests/Smoke.ProCore.ContractTests/ApiContractTests.cs`
- `g/pro-core/tests/Smoke.ProCore.ContractTests/IntegrationEventContractTests.cs`
- `g/pro-core/tests/Smoke.ProCore.ContractTests/InboxContractTests.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisOpenApiServiceCollectionExtensions.cs`
- `g/pro-core/src/Smoke.ProCore.BuildingBlocks/Events/IntegrationEvent.cs`
- `g/pro-core/src/Smoke.ProCore.Modules/Modules/WorkItems/Contracts/WorkItemCreatedIntegrationEvent.cs`

Representative TaskHub output:

- `g/taskhub/tests/Aegis.TaskHub.ContractTests/ApiContractTests.cs`
- `g/taskhub/src/Aegis.TaskHub.Modules/Modules/Tasks/Contracts/TaskCreatedIntegrationEvent.cs`

Representative core exclusion:

- `g/core-core/tests/Smoke.CoreCore.ContractTests` is absent.
- `g/core-core/Smoke.CoreCore.sln` does not include `ContractTests`.
- `g/core-core/docs/contracts.md` states core does not generate the pro/advanced contract test project.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` before full smoke | Pass |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq6y1hqg-4c09fc52`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq6yhsor-2804b561`. |
| `npm run check` after writing this report | First attempt timed out at 120 seconds; rerun with a longer timeout passed. |

Generated smoke observations:

- Core architecture tests passed with 35 tests.
- Pro/advanced architecture tests passed with 35 tests.
- Pro/advanced contract tests passed with 11 tests.
- Pro/advanced integration tests passed with 8 tests and 1 skipped Docker-gated Testcontainers test.
- Generated contract tests ran for both core and MediatR mediator variants.

## OpenQuestions.md Updates

`OpenQuestions.md` was updated with P1D-3A inferred decisions:

- `Q-20260609-008`: use semantic OpenAPI and endpoint metadata assertions instead of full OpenAPI snapshots.
- `Q-20260609-009`: add lightweight integration event type/version metadata.
- `Q-20260609-010`: generate contract test foundation only for pro and advanced.

Open blockers from `OpenQuestions.md`: none.

## Remaining P1D-3B/P1D-4/P2 Gaps

Remaining work is outside P1D-3A:

- P1D-3B: performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Validation Limitations

Docker-backed Testcontainers execution was not run because default validation intentionally does not require Docker. Generated Docker tests remain skipped unless `AEGIS_RUN_TESTCONTAINERS=true` is set.

No broker, external identity provider, real JWT issuer, or external service was used. Contract tests verify generated metadata and serialization boundaries, not a deployed production environment.
