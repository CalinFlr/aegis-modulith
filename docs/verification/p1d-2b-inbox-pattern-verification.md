# P1D-2B Inbox Pattern Verification

Date: 2026-06-09

## Summary

P1D-2B is confirmed closed.

Implemented a generated inbox/idempotency foundation for `pro` and `advanced` profiles. Core remains lightweight and excludes active inbox infrastructure.

No P1D-3 contract/performance tests, P1D-4 deployment skeleton, UI, event sourcing, broker dependency, public screenshots, badges, docs site work, P2 public polish, project rename, or architecture redesign was added.

Fresh validation evidence:

- Targeted pro output: `artifacts/p1d-2b-targeted/pro`
- Targeted TaskHub output: `artifacts/p1d-2b-targeted/taskhub`
- First full smoke run: `artifacts/template-smoke/runs/mq6rx3yi-2121f4a2`
- Immediate second full smoke run: `artifacts/template-smoke/runs/mq6s8o50-c34631b7`
- Primary generated evidence source: `artifacts/template-smoke/runs/mq6s8o50-c34631b7`

## Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced inbox persistence model | Pass | Generated pro/advanced API includes `Pro/Infrastructure/Inbox/InboxMessage.cs` with `MessageId`, `IdempotencyKey`, `MessageType`, `Payload`, `Status`, `ReceivedAtUtc`, `ProcessedAtUtc`, `FailureReason`, `AttemptCount`, `LockToken`, and `LockedUntilUtc`. |
| EF Core persistence integration | Pass | Generated `AegisInboxDbContext` exposes `DbSet<InboxMessage>` and uses schema `integration`; `InboxMessageConfiguration` maps `integration.inbox_messages`. |
| Idempotency uniqueness | Pass | Generated EF configuration has unique indexes on `MessageId` and `IdempotencyKey`. Smoke asserts both markers. |
| Inbox service abstraction | Pass | Generated `IInboxStore` includes `AcceptAsync`, `IsDuplicateAsync`, `GetPendingAsync`, `TryBeginProcessingAsync`, `MarkProcessedAsync`, and `MarkFailedAsync`. |
| Duplicate and already-processed behavior | Pass | Generated `EfCoreInboxStore` returns `Accepted`, `Duplicate`, and `AlreadyProcessed`; generated tests cover first acceptance, duplicate detection, and processed-message idempotency. |
| Processor scaffold | Pass | Generated `InboxProcessor`, `IInboxMessageHandler`, `InboxProcessorWorker`, and `SampleIntegrationEventInboxHandler` exist. |
| Hosted processor safety | Pass | `InboxProcessorWorker` registration is gated by `Inbox:EnableBackgroundProcessor`; default validation does not start a broker or poll external services. |
| Integration-event contract usage | Pass | Sample handler uses generated module `Contracts` integration-event types and does not reference Domain namespaces. Starter output uses `WorkItemCreatedIntegrationEvent`; TaskHub output uses `TaskCreatedIntegrationEvent`. |
| No broker dependency | Pass | Generated architecture tests and smoke assertions check no MassTransit, RabbitMQ, Kafka, Azure Service Bus, or NServiceBus dependency markers are present by default. |
| Core exclusion | Pass | Core generated outputs exclude `src/<App>.Api/Pro/Infrastructure/Inbox`, EF InMemory inbox test package versions, API direct inbox persistence references, and generated inbox tests. |
| Generated tests | Pass | Pro/advanced integration tests include `Inbox/InboxStoreTests.cs` covering first message accepted, duplicate detection, processed not accepted again, failure state, and handler invoked once for duplicate inputs. |
| No Docker or broker required | Pass | Inbox behavior tests use EF InMemory. Default smoke passed while Docker was unavailable and no broker was configured. |
| Smoke assertions | Pass | Root `tools/guardrails/check.mjs` includes `assertP1D2BInboxSemantics` and invokes it for every smoke variant. |
| Docs and acceptance | Pass | `docs/messaging.md`, generated `docs/messaging.md`, `docs/testing.md`, `docs/architecture.md`, acceptance files, and spec task/acceptance files were updated. |
| OpenQuestions state | Pass | `OpenQuestions.md` records inferred decisions for database uniqueness, broker omission, and pro/advanced-only active inbox. No blockers were added. |

## Generated Output Evidence

Representative pro output from `mq6s8o50-c34631b7`:

- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxMessage.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxMessageConfiguration.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/AegisInboxDbContext.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/IInboxStore.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/EfCoreInboxStore.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxProcessor.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/InboxProcessorWorker.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Infrastructure/Inbox/SampleIntegrationEventInboxHandler.cs`
- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/Inbox/InboxStoreTests.cs`

Representative TaskHub output from targeted validation:

- `artifacts/p1d-2b-targeted/taskhub/src/Verify.P1D2B.TaskHub.Api/Pro/Infrastructure/Inbox/SampleIntegrationEventInboxHandler.cs`
- `artifacts/p1d-2b-targeted/taskhub/tests/Verify.P1D2B.TaskHub.IntegrationTests/Inbox/InboxStoreTests.cs`

Representative core output from `mq6s8o50-c34631b7`:

- `g/core-core/src/Smoke.CoreCore.Api/Pro/Infrastructure/Inbox` is absent.
- `g/core-core/tests/Smoke.CoreCore.IntegrationTests` is absent.
- `g/core-core/docs/messaging.md` states core does not generate active inbox infrastructure.

## Generated Tests

Generated pro/advanced integration tests now include:

- `First_message_is_accepted`
- `Duplicate_message_id_is_detected`
- `Processed_message_is_not_accepted_again`
- `Failed_message_records_failure_and_retry_state`
- `Processor_invokes_handler_once_for_duplicate_inputs`

Targeted generated validation:

| Output | Result |
| --- | --- |
| `artifacts/p1d-2b-targeted/pro` | Build passed; architecture tests passed 31; integration tests passed 8 and skipped 1 Docker-gated test. |
| `artifacts/p1d-2b-targeted/taskhub` | Build passed; architecture tests passed 31; integration tests passed 8 and skipped 1 Docker-gated test. |

## Smoke Assertions Added

`tools/guardrails/check.mjs` now asserts:

- pro/advanced generated outputs include inbox entity, status enum, EF configuration, DbContext, store abstraction, EF store, processor, opt-in worker, sample handler, and generated tests;
- pro/advanced EF configuration includes unique `MessageId` and `IdempotencyKey` indexes;
- pro/advanced service registration wires `AddAegisInbox(configuration)`;
- generated inbox tests cover duplicate/idempotency behavior and single handler invocation;
- generated messaging docs are profile-accurate;
- core generated output excludes active inbox infrastructure;
- generated output has no unresolved template directives;
- no broker dependency is generated by default.

## Checks Run

| Command | Result |
| --- | --- |
| `dotnet pack templates/Aegis.Modulith.Templates/Aegis.Modulith.Templates.csproj -c Release -o artifacts/p1d-2b-targeted/packages` | Pass |
| Targeted pro `dotnet restore`, `dotnet build -c Release --no-restore`, `dotnet test -c Release --no-build` | Pass |
| Targeted TaskHub `dotnet restore`, `dotnet build -c Release --no-restore`, `dotnet test -c Release --no-build` | Pass |
| `npm run check` | Pass |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq6rx3yi-2121f4a2`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq6s8o50-c34631b7`. |

Final `npm run check` is run after this report is written and should be recorded in the final response.

## Validation Limitations

Docker is not available on `PATH`, so live Testcontainers execution was not run.

This is acceptable for P1D-2B because default validation must not require Docker. Generated Docker-backed tests remain opt-in through `AEGIS_RUN_TESTCONTAINERS=true`.

No external broker, queue, webhook provider, or identity provider was used or required. The inbox processor is scaffolded and the hosted worker is disabled unless `Inbox:EnableBackgroundProcessor=true`.

## OpenQuestions.md Updates

`OpenQuestions.md` was updated with inferred decisions:

- `Q-20260609-005`: use database uniqueness for inbox idempotency.
- `Q-20260609-006`: omit concrete broker integration from the inbox scaffold.
- `Q-20260609-007`: generate active inbox only for pro and advanced.

Open blockers from `OpenQuestions.md`: none.

## Remaining Gaps

Remaining work is outside P1D-2B:

- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Closure

P1D-2B is confirmed closed.

Recommended next step: start P1D-3 contract/performance tests as a separate goal when requested.
