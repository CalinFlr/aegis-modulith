# P1D-3A Post-Fix Verification

Date: 2026-06-10

## Summary

P1D-3A is confirmed closed after a verification-only run against the generated contract test implementation.

No implementation fixes were made. This run did not start P1D-3B performance smoke tests, P1D-4 deployment skeleton work, UI work, event sourcing, broker integration, public screenshots, badges, docs-site work, P2 public polish, or project renaming.

Fresh validation evidence:

- First smoke run: `artifacts/template-smoke/runs/mq7kp5bz-ed81bf1c`.
- Immediate second smoke run: `artifacts/template-smoke/runs/mq7l2wia-d557578d`.
- Primary generated evidence source: `artifacts/template-smoke/runs/mq7l2wia-d557578d`.

## P1D-3A Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced contract test project | Pass | `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `strict-enterprise`, and `taskhub` outputs include `tests/<App>.ContractTests`. Core and non-pro/advanced option outputs do not. |
| Contract test project in generated solution | Pass | Generated pro, advanced, and TaskHub `.sln` files include the matching `ContractTests` project. Core `.sln` does not. |
| OpenAPI document production | Pass | `ApiContractTests.OpenApi_document_can_be_produced_and_declares_jwt_bearer_security_scheme` starts `WebApplicationFactory<Program>()` and requests `/openapi/v1.json`. |
| Semantic route/method/status/content checks | Pass | `ApiContractTests.Expected_routes_methods_status_codes_and_content_types_are_declared` checks operations by route, method, status code, request content, response content, and bearer requirement. No full OpenAPI snapshot is used. |
| JWT bearer OpenAPI metadata | Pass | API contract tests check the `Bearer` security scheme type, scheme, and bearer format. Smoke assertions also check `BearerSecuritySchemeTransformer`, `AddOperationTransformer`, `IAuthorizeData`, and `JwtBearerDefaults.AuthenticationScheme`. |
| Named permission-policy metadata | Pass | API contract tests inspect endpoint metadata and assert `AegisAuthorizationPolicies` constants. They also assert registered named policies exist. |
| Fake auth excluded from production contract | Pass | API contract tests assert OpenAPI JSON and production API source do not contain `Aegis.Test`, `FakeAuthentication`, or `X-Test-*` markers. Architecture boundary tests repeat the production-source check. |
| Starter and TaskHub endpoint coverage | Pass | Starter outputs check `/work-items/` and `/work-items/{id}`. TaskHub checks `/tasks/` GET/POST with task read/write policies. Advanced additionally checks `/operations/advanced`. |
| Integration event metadata | Pass | Generated `IntegrationEventContractAttribute` and `IntegrationEventContractMetadata` exist in BuildingBlocks; generated integration event records declare stable type/version metadata. |
| Integration event JSON round trips | Pass | `IntegrationEventContractTests.Integration_event_contracts_round_trip_with_system_text_json` serializes and deserializes each integration event with `System.Text.Json` and compares public properties. |
| Required event properties | Pass | Generated event records include `Id`, event-specific identifiers, and `OccurredAtUtc`; the round-trip test compares public constructor-backed properties for each event type. |
| Domain/integration event distinction | Pass | Contract tests assert domain events are not integration events, do not live under `.Contracts`, and do not carry `IntegrationEventContractAttribute`. |
| Integration contract location | Pass | Contract tests assert integration event files live under `Contracts` folders and namespaces contain `.Contracts`. |
| Inbox handler contract boundary | Pass | Starter and TaskHub sample inbox handlers consume integration event contracts and use `IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>()`; they do not consume domain entities. |
| Inbox payload and identity contract | Pass | `InboxContractTests` verifies serialized payload, `MessageId`, `IdempotencyKey`, and integration event message type. |
| No broker dependency | Pass | Contract tests and smoke assertions deny broker markers such as MassTransit, RabbitMQ, Kafka, Azure Service Bus, and NServiceBus. Production generated outputs contain no broker dependency markers. |
| No event sourcing | Pass | Metadata is documented as lightweight contract metadata only. Production generated outputs contain no event store or event sourcing markers. |
| Core remains lightweight | Pass | Core excludes `tests/<App>.ContractTests`, keeps generated docs explaining the exclusion, and has no `Microsoft.AspNetCore.Mvc.Testing` or contract-test project in its solution. Existing architecture-test packages remain expected. |
| Dependency boundaries | Pass | Production projects do not reference contract tests; integration contracts do not depend on Infrastructure, EF Core, Npgsql, `DbContext`, or inbox entities. |
| P1C architecture tests | Pass | Generated architecture tests ran in smoke with 35 passing tests per generated architecture-test project. |
| P1D-1/P1D-2A/P1D-2B behavior | Pass | Full smoke still runs generated architecture, integration, permission, inbox, and contract tests. Docker-gated Testcontainers tests remain skipped by default. |
| Smoke semantic assertions | Pass | `tools/guardrails/check.mjs` includes `assertP1D3AContractTestSemantics` covering project shape, solution inclusion, OpenAPI/auth-policy markers, integration-event markers, inbox markers, core exclusion, docs, no Docker/Testcontainers in contract tests, and unresolved-token checks. |
| Default validation independence | Pass | Required validation passed without Docker, a broker, external identity provider, real JWT issuer, or external services. |

## Generated Output Evidence

Primary evidence came from `artifacts/template-smoke/runs/mq7l2wia-d557578d`.

Contract test project shape:

- `g/pro-core/tests/Smoke.ProCore.ContractTests/Smoke.ProCore.ContractTests.csproj`
- `g/pro-mediatr/tests/Smoke.ProMediatR.ContractTests/Smoke.ProMediatR.ContractTests.csproj`
- `g/advanced-core/tests/Smoke.AdvancedCore.ContractTests/Smoke.AdvancedCore.ContractTests.csproj`
- `g/advanced-mediatr/tests/Smoke.AdvancedMediatR.ContractTests/Smoke.AdvancedMediatR.ContractTests.csproj`
- `g/strict-enterprise/tests/Smoke.StrictEnterprise.ContractTests/Smoke.StrictEnterprise.ContractTests.csproj`
- `g/taskhub/tests/Aegis.TaskHub.ContractTests/Aegis.TaskHub.ContractTests.csproj`

Core exclusion evidence:

- `g/core-core/tests/Smoke.CoreCore.ContractTests` is absent.
- `g/core-mediatr/tests/Smoke.CoreMediatR.ContractTests` is absent.
- `g/core-core/Smoke.CoreCore.sln` does not include `ContractTests`.
- `g/core-core/docs/contracts.md` states core does not generate the pro/advanced contract test project.

Generated solution evidence:

- `g/pro-core/Smoke.ProCore.sln` includes `tests\Smoke.ProCore.ContractTests\Smoke.ProCore.ContractTests.csproj`.
- `g/advanced-core/Smoke.AdvancedCore.sln` includes `tests\Smoke.AdvancedCore.ContractTests\Smoke.AdvancedCore.ContractTests.csproj`.
- `g/taskhub/Aegis.TaskHub.sln` includes `tests\Aegis.TaskHub.ContractTests\Aegis.TaskHub.ContractTests.csproj`.

## API Contract Test Evidence

Representative file: `g/pro-core/tests/Smoke.ProCore.ContractTests/ApiContractTests.cs`.

Verified behavior:

- Produces `/openapi/v1.json` through `WebApplicationFactory<Program>()`.
- Checks `components.securitySchemes.Bearer` for `type=http`, `scheme=bearer`, and `bearerFormat=JWT`.
- Checks routes and methods through `AssertOperation(...)` instead of a full OpenAPI snapshot.
- Checks declared status codes and `application/json` request/response content where generated endpoints declare bodies.
- Checks protected operations expose bearer security.
- Checks endpoint metadata with `EndpointDataSource`, `IHttpMethodMetadata`, and `IAuthorizeData`.
- Checks named policy constants from `AegisAuthorizationPolicies`, including WorkItems, Tasks, Operations, and Advanced policy names.
- Checks fake-auth markers are absent from OpenAPI JSON and production API source.

Representative route coverage:

- Starter: `/`, `/operations/outbox`, `/work-items/`, `/work-items/{id}`.
- Advanced: `/operations/advanced`.
- TaskHub: `/tasks/` GET and POST, with `TasksRead` and `TasksWrite` policy checks.

The tests are semantic and do not store brittle full OpenAPI snapshots.

## Integration Event Contract Test Evidence

Representative file: `g/pro-core/tests/Smoke.ProCore.ContractTests/IntegrationEventContractTests.cs`.

Verified behavior:

- Finds generated integration event types assignable to `IntegrationEvent`.
- Requires `IntegrationEventContractAttribute` on every integration event.
- Requires non-empty type metadata and version `>= 1`.
- Requires integration events to live under `.Contracts` and not under `.Domain`.
- Verifies integration event source files live in `Contracts` folders.
- Serializes and deserializes integration events with `System.Text.Json`.
- Compares all public instance properties after round trip.
- Confirms domain events remain distinct from integration events and have no integration contract metadata.
- Confirms the sample inbox handler uses integration contract metadata and contract namespaces, not domain entities.

Representative production metadata:

- `g/pro-core/src/Smoke.ProCore.BuildingBlocks/Events/IntegrationEvent.cs` defines `IntegrationEventContractAttribute` and `IntegrationEventContractMetadata`.
- `g/pro-core/src/Smoke.ProCore.Modules/Modules/WorkItems/Contracts/WorkItemCreatedIntegrationEvent.cs` declares `[IntegrationEventContract("work-items.created", 1)]`.
- `g/taskhub/src/Aegis.TaskHub.Modules/Modules/Tasks/Contracts/TaskCreatedIntegrationEvent.cs` declares `[IntegrationEventContract("tasks.created", 1)]`.

This is lightweight type/version metadata for contract drift checks. It is not event sourcing.

## Permission And Inbox Contract Evidence

Permission/auth evidence:

- `ApiContractTests.Permission_policy_constants_are_registered_as_named_policies` checks generated policy registration.
- `ApiContractTests.Protected_endpoints_expose_named_permission_policy_metadata` checks endpoint metadata uses named policies.
- Contract tests check fake auth is absent from production API contracts.
- Generated `docs/authentication.md` explains fake authentication is test-only and does not imply production behavior.
- Production generated outputs contain no identity provider dependency markers such as IdentityServer, OpenIddict, Duende, Keycloak, or `AddIdentity`.

Inbox evidence:

- `InboxContractTests.Inbox_message_payload_contract_is_serializable_and_keeps_message_identity` checks `MessageId`, `IdempotencyKey`, message type, and serialized payload round trip.
- `InboxContractTests.Inbox_contract_tests_do_not_require_broker_dependencies_or_exactly_once_claims` checks no broker dependency markers and no broker-level exactly-once claims.
- `SampleIntegrationEventInboxHandler` in starter and TaskHub consumes integration event contracts and uses `IntegrationEventContractMetadata.TypeName<SampleIntegrationEvent>()`.
- Generated `docs/messaging.md` documents the inbox payload contract and idempotency fields without claiming broker exactly-once delivery.

## Dependency Boundary Evidence

Generated `ContractBoundaryTests` assert:

- Core does not include contract tests; pro/advanced do.
- Production projects do not reference contract tests.
- Integration contracts do not depend on Infrastructure, EF Core, Npgsql, `DbContext`, or `InboxMessage`.
- Production API source does not reference fake-auth test infrastructure.

Additional generated-source checks:

- `rg` found no broker, event sourcing, or external identity provider dependency markers in pro/advanced production source and package props.
- Contract test projects reference production API, BuildingBlocks, and Modules projects, but production project files do not reference `ContractTests`.
- Core retains only normal architecture-test dependencies and does not include the pro/advanced `Microsoft.AspNetCore.Mvc.Testing` contract-test project.

## Smoke Assertions Reviewed

Root smoke assertions in `tools/guardrails/check.mjs` were reviewed.

`assertP1D3AContractTestSemantics` checks:

- core excludes the contract test project and documents the exclusion;
- pro/advanced include `tests/<App>.ContractTests`;
- generated solutions include contract tests so `dotnet test` runs them;
- contract tests reference API, BuildingBlocks, and Modules;
- contract tests use `Microsoft.AspNetCore.Mvc.Testing` and do not require `Testcontainers.PostgreSql`;
- production projects do not reference contract tests;
- OpenAPI bearer metadata is wired through `AddAegisOpenApi`, `BearerSecuritySchemeTransformer`, `AddOperationTransformer`, `IAuthorizeData`, and `JwtBearerDefaults.AuthenticationScheme`;
- integration event metadata exists;
- API contract tests contain OpenAPI, route, method, status, content-type, bearer, named-policy, fake-auth exclusion, starter, TaskHub, and advanced endpoint markers;
- integration event tests contain metadata, JSON round-trip, contract-folder, and domain/integration separation markers;
- inbox tests contain payload identity, `MessageId`, `IdempotencyKey`, broker-deny, and exactly-once-deny markers;
- generated contract docs describe the project and default runtime independence;
- generated contract test files contain no unresolved template tokens.

The generated-output directive scan also ran during both smoke runs through `assertNoTemplateDirectives`.

Manual generated-output scan result for the second smoke run:

```text
NO_UNRESOLVED_TEMPLATE_TOKENS_IN_CONTRACT_TESTS
```

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass. |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq7kp5bz-ed81bf1c`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq7l2wia-d557578d`. |
| `npm run check` after writing this report | Pass. |

Generated smoke observations:

- Core architecture tests passed with 35 tests.
- Pro and advanced architecture tests passed with 35 tests.
- Pro and advanced contract tests passed with 11 tests.
- Pro and advanced integration tests passed with 8 tests and 1 skipped Docker-gated Testcontainers test.
- Generated contract tests ran for both core and MediatR mediator variants where pro/advanced contract tests are intended.
- Smoke remained idempotent across two immediate runs.

## P1D-3A Closure

P1D-3A is confirmed closed.

The generated contract test foundation is meaningful for API contract drift, integration event contract drift, permission/auth metadata drift, inbox payload/idempotency contract drift, and production/test boundary drift. It remains scoped to `pro` and `advanced` outputs and does not require Docker, a broker, an external identity provider, a real JWT issuer, or external services.

## Remaining P1D-3B/P1D-4/P2 Gaps

The following remain outside P1D-3A and were not started:

- P1D-3B: performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Validation Limitations

Docker-backed Testcontainers execution was not run because default validation intentionally does not require Docker. Generated Docker tests remain skipped unless `AEGIS_RUN_TESTCONTAINERS=true` is set.

No broker, external identity provider, real JWT issuer, or external service was used. Contract tests verify generated metadata, route semantics, serialization boundaries, and dependency boundaries rather than a deployed production environment.

No validation command timed out in this run.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

Open blockers from `OpenQuestions.md`: none.

Relevant P1D-3A inferred decisions are present and accurate:

- `Q-20260609-008`: semantic OpenAPI assertions over full snapshots.
- `Q-20260609-009`: lightweight integration event type/version metadata without event sourcing.
- `Q-20260609-010`: contract tests generated for pro/advanced only.

The file has 15 inferred decisions. A separate count of 16 `Q-` headings includes the commented entry template, not an active decision. There is no inconsistent inferred-decision summary to correct.

## Spec Folder Used

Reviewed but not updated:

- `specs/0001-aegis-template-core/spec.md`
- `specs/0001-aegis-template-core/acceptance.md`

## Recommended Next Step

Start P1D-3B performance smoke tests as a separate goal when requested.
