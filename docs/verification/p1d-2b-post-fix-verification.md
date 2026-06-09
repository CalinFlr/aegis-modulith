# P1D-2B Post-Fix Verification

Date: 2026-06-09

## Summary

P1D-2B is confirmed closed after a fresh verification-only run.

No implementation fixes were made. This run did not start P1D-3 contract/performance tests, P1D-4 deployment skeleton work, UI, event sourcing, broker integration, public screenshots, badges, docs-site work, P2 public polish, or project renaming.

Fresh validation evidence:

- First smoke run: `artifacts/template-smoke/runs/mq6w3rjc-48446615`
- Immediate second smoke run: `artifacts/template-smoke/runs/mq6wdz3i-68df7d77`
- Primary generated evidence source: `artifacts/template-smoke/runs/mq6wdz3i-68df7d77`

## P1D-2B Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Inbox persistence model | Pass | Generated pro and advanced outputs include `Pro/Infrastructure/Inbox/InboxMessage.cs` with `MessageId`, `IdempotencyKey`, `MessageType`, `Payload`, `Status`, `ReceivedAtUtc`, `ProcessedAtUtc`, `FailureReason`, `AttemptCount`, `LockToken`, and `LockedUntilUtc`. |
| EF Core persistence integration | Pass | Generated `AegisInboxDbContext` exposes `DbSet<InboxMessage>`; `InboxMessageConfiguration` maps `integration.inbox_messages`. |
| Idempotency uniqueness | Pass | Generated EF configuration contains unique indexes for both `MessageId` and `IdempotencyKey`. |
| PostgreSQL-friendly naming | Pass | Generated configuration uses lower snake-case table name `inbox_messages` in schema `integration`. |
| No cross-module foreign keys | Pass | Inbox configuration contains no FK configuration; generated architecture tests continue to assert no generated FK configuration by default. |
| Core exclusion | Pass | Latest `core-core` output has no `src/Smoke.CoreCore.Api/Pro/Infrastructure/Inbox` directory and no integration test project. Core docs explicitly state active inbox infrastructure is excluded. |
| Inbox store abstraction | Pass | `IInboxStore` exposes accept, duplicate, pending, begin-processing, processed, and failed transitions with cancellation tokens. |
| Idempotent acceptance | Pass | `EfCoreInboxStore` returns `Accepted`, `Duplicate`, or `AlreadyProcessed`; duplicate behavior is backed by uniqueness and tested. |
| Logging and cancellation | Pass | Store and processor methods accept `CancellationToken`; store/processor use `ILogger` for accepted, duplicate, processed, warning, and dispatch-failure paths. |
| Processor scaffold | Pass | Generated `InboxProcessor`, `IInboxMessageHandler`, `InboxProcessorWorker`, and `SampleIntegrationEventInboxHandler` exist in pro/advanced. |
| Opt-in background worker | Pass | `InboxProcessorWorker` is registered only when `Inbox:EnableBackgroundProcessor` is true. |
| Handler contract boundary | Pass | Starter handler uses `Modules.WorkItems.Contracts.WorkItemCreatedIntegrationEvent`; TaskHub handler uses `Modules.Tasks.Contracts.TaskCreatedIntegrationEvent`; generated architecture tests assert `.Contracts` usage and no `.Domain` usage. |
| No broker dependency | Pass | Generated production source and package props contain no MassTransit, RabbitMQ, Kafka, Azure Service Bus, NServiceBus, or Confluent markers. Broker names only appear in generated architecture-test deny-lists. |
| No event sourcing | Pass | Docs state the inbox is not event sourcing; no EventStore/event-sourcing implementation was added. |
| Generated tests | Pass | Generated pro/advanced tests cover first acceptance, duplicate detection, processed-message idempotency, failed-message state, and single handler invocation for duplicate inputs. |
| Default validation independence | Pass | Inbox tests use EF InMemory and do not require Docker, a broker, external service, or identity provider. |
| Previous behavior intact | Pass | Two smoke runs passed across core/pro/advanced, core and MediatR variants, item templates, P1A AI/guardrail/docs/license semantics, P1B item templates, P1C architecture tests, P1D-1 Testcontainers/fake-auth/resilience, and P1D-2A auth/permissions. |
| Documentation and acceptance | Pass | `docs/messaging.md`, `docs/testing.md`, `docs/architecture.md`, `docs/acceptance-criteria.md`, and `specs/0001-aegis-template-core/acceptance.md` describe P1D-2B accurately. |
| OpenQuestions state | Pass | `OpenQuestions.md` contains `Q-20260609-005`, `Q-20260609-006`, and `Q-20260609-007`; no new blocker or inferred decision was found. |

## Generated Output Evidence

Representative pro output from `mq6wdz3i-68df7d77`:

- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxMessage.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxMessageStatus.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxMessageConfiguration.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/AegisInboxDbContext.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/IInboxStore.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/EfCoreInboxStore.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxProcessor.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxProcessorWorker.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/IInboxMessageHandler.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/SampleIntegrationEventInboxHandler.cs`
- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/Inbox/InboxStoreTests.cs`

Representative advanced output:

- `g/advanced-core/src/Smoke.AdvancedCore.Api/Pro/Infrastructure/Inbox/InboxMessage.cs`
- `g/advanced-core/tests/Smoke.AdvancedCore.IntegrationTests/Inbox/InboxStoreTests.cs`

Representative core exclusion:

- `g/core-core/src/Smoke.CoreCore.Api/Pro/Infrastructure/Inbox` is absent.
- `g/core-core/tests/Smoke.CoreCore.IntegrationTests` is absent.
- `g/core-core/docs/messaging.md` says core does not generate active inbox infrastructure.

## Inbox Idempotency Evidence

Generated persistence fields in `InboxMessage.cs` include stable message identity, serialized payload placeholder, state, timestamps, failure state, retry count, and simple lease fields.

Generated `InboxMessageConfiguration.cs` contains:

- `ToTable("inbox_messages", AegisInboxDbContext.Schema)`
- `HasIndex(message => message.MessageId).IsUnique()`
- `HasIndex(message => message.IdempotencyKey).IsUnique()`
- `HasIndex(message => new { message.Status, message.ReceivedAtUtc })`

Generated behavior tests in `InboxStoreTests.cs` include:

- `First_message_is_accepted`
- `Duplicate_message_id_is_detected`
- `Processed_message_is_not_accepted_again`
- `Failed_message_records_failure_and_retry_state`
- `Processor_invokes_handler_once_for_duplicate_inputs`

The tests use `UseInMemoryDatabase(...)`, so default validation does not require Docker or a broker.

## Architecture Boundary Evidence

The inbox lives under generated API `Pro/Infrastructure/Inbox`, which is the documented pro/advanced cross-cutting infrastructure area for this scaffold. Generated module domain code does not reference inbox infrastructure.

Generated handlers consume integration-event contracts, not domain entities:

- Starter pro handler imports `Smoke.ProCore.Modules.Modules.WorkItems.Contracts` and aliases `WorkItemCreatedIntegrationEvent`.
- TaskHub handler imports `Aegis.TaskHub.Modules.Modules.Tasks.Contracts` and aliases `TaskCreatedIntegrationEvent`.

Generated `InboxArchitectureTests.cs` asserts:

- profile-specific inbox wiring;
- unique message identity persistence;
- domain code does not reference inbox infrastructure;
- inbox processing uses integration-event contracts;
- no broker dependency is generated by default.

P1D-2A and P1D-1 boundaries remain intact in the latest generated output:

- Production pro/advanced `src` contains no `FakeAuthentication`, `Aegis.Test`, or `X-Test-*` markers.
- Testcontainers markers are absent from production `src`; Docker-backed tests remain in generated test projects and are gated by `AEGIS_RUN_TESTCONTAINERS=true`.
- Pro `Program.cs` still calls `UseAuthentication()` before `UseAuthorization()`.
- Permission policy usage remains through named `AegisAuthorizationPolicies` constants.

## Smoke Assertions Reviewed

Root `tools/guardrails/check.mjs` contains `assertP1D2BInboxSemantics` and calls it for generated variants. The assertions cover:

- pro/advanced inbox entity, status enum, EF configuration, DbContext, store abstraction, EF store, processor, worker, handler, and generated tests;
- unique `MessageId` and `IdempotencyKey` indexes;
- `AddAegisInbox(configuration)` service wiring;
- `Inbox:EnableBackgroundProcessor` opt-in worker registration;
- duplicate/idempotency test method markers;
- generated messaging docs profile accuracy;
- core exclusion of active inbox infrastructure;
- no generated broker dependency by default.

The latest generated-output directive scan returned:

```text
NO_UNRESOLVED_TEMPLATE_DIRECTIVES
```

The two smoke runs also prove idempotent generation by producing separate fresh run directories and passing the matrix twice.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq6w3rjc-48446615`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq6wdz3i-68df7d77`. |
| `npm run check` after writing this report | Pass |
| `npm run check` after recording the post-report result | Pass |

Smoke observations:

- Core variants ran generated architecture tests with 31 passed.
- Pro and advanced variants ran generated architecture tests with 31 passed.
- Pro and advanced integration tests reported 8 passed and 1 skipped Docker-gated Testcontainers test.
- Core and MediatR mediator variants built and tested successfully.

## Documentation And Acceptance

Root and generated messaging docs are truthful:

- They explain the inbox purpose as a database-backed idempotency foundation.
- They explain `MessageId` and `IdempotencyKey`.
- They explain duplicate handling, processed-message idempotency, retry/failure state, and the opt-in background processor.
- They document where a real broker, webhook, queue consumer, or import endpoint would call `IInboxStore.AcceptAsync(...)`.
- They state no broker is included by default.
- They state the inbox is not event sourcing.
- They do not claim broker-level exactly-once delivery.

Acceptance files accurately mark P1D-2B complete:

- `docs/acceptance-criteria.md`
- `specs/0001-aegis-template-core/acceptance.md`

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed in this verification run.

Required P1D-2B inferred decisions are present:

- `Q-20260609-005`: use database uniqueness for inbox idempotency.
- `Q-20260609-006`: omit concrete broker integration from the inbox scaffold.
- `Q-20260609-007`: generate active inbox only for pro and advanced.

Open blockers from `OpenQuestions.md`: none.

Existing inferred assumptions remain:

- `Q-20260606-001` through `Q-20260609-007`, including project naming, spec-driven development, module manifests, MediatR pinning, generated AI skills, drop-in item templates, opt-in Testcontainers, test-only fake authentication, JWT/permission scaffold defaults, and the P1D-2B inbox defaults above.

## Remaining P1D-3/P1D-4/P2 Gaps

Remaining work is outside P1D-2B:

- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Validation Limitations

Docker is not available on `PATH`, so live Testcontainers execution was not run. This is acceptable for P1D-2B because default validation must not require Docker and the Docker-backed test is intentionally skipped unless `AEGIS_RUN_TESTCONTAINERS=true`.

No external broker, webhook provider, external service, or identity provider was used. The verification covered generated scaffold behavior, generated tests, docs, and smoke assertions rather than a real inbound broker integration.

## Closure

P1D-2B remains confirmed closed.

Recommended next goal: start P1D-3 contract/performance tests as a separate goal when requested.
