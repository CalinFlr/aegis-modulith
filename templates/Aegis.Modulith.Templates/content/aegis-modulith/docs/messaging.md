# Messaging

#if (profile == "core")
## Core Profile

Core does not generate active inbox infrastructure. It keeps the API and module skeleton lightweight and leaves inbound message idempotency for `pro` or `advanced`.

Core still uses integration event contracts when item templates or future modules need to describe messages, but it does not include `InboxMessage`, `AegisInboxDbContext`, `IInboxStore`, an inbox processor, or generated inbox tests.
#else
## Pro And Advanced Profiles

Generated `pro` and `advanced` outputs include an inbox pattern scaffold for idempotent inbound integration event and message processing.

This is a database-backed idempotency foundation. It is not event sourcing, and it does not include a broker by default.

## Purpose

A real broker, webhook, queue consumer, or import endpoint should first call `IInboxStore.AcceptAsync(...)` with the stable `MessageId`, the `MessageType`, and the serialized payload.

The generated store also derives an `IdempotencyKey` from `MessageId` and persists it with a uniqueness constraint. If the same message arrives again, the store returns `Duplicate` or `AlreadyProcessed` instead of inserting another row.

## Persistence

The generated inbox uses `AegisInboxDbContext`, `InboxMessage`, EF Core configuration for `integration.inbox_messages`, unique indexes on `MessageId` and `IdempotencyKey`, and transparent state fields: `Pending`, `Processing`, `Processed`, `Failed`, `ReceivedAtUtc`, `ProcessedAtUtc`, `FailureReason`, and `AttemptCount`.

Inbox payloads are serialized integration event messages. Do not store Domain entities as inbox payloads.

Generated integration event records declare `IntegrationEventContractAttribute` with a stable type name and positive integer version. The sample inbox handler uses that metadata as its `MessageType`. This is a contract-testing convention, not event sourcing.

## Processor Execution

The generated `InboxProcessor` dispatches pending rows to registered `IInboxMessageHandler` implementations. The sample handler uses generated integration event contracts from module `Contracts` folders.

The hosted background processor is opt-in:

```json
{
  "Inbox": {
    "EnableBackgroundProcessor": true
  }
}
```

Leave `Inbox:EnableBackgroundProcessor` disabled until the application has a real database migration and an explicit inbound consumer path. No broker is included by default.

## Retry And Failure State

Processing changes a row from `Pending` or `Failed` to `Processing`, increments `AttemptCount`, and applies a short lease marker. Successful handling marks the row `Processed` and records `ProcessedAtUtc`.

Handler failures mark the row `Failed` and store `FailureReason`. The simple processor can pick up failed rows again on a later pass.

## Tests

Generated inbox tests live under `tests/Aegis.Template.IntegrationTests/Inbox`.

They prove first-message acceptance, duplicate detection, processed-message idempotency, failure state, and single handler invocation for duplicate inputs. These tests use EF InMemory and do not require Docker.

Generated contract tests also verify integration event serialization, event type/version metadata, inbox payload identity fields, and the absence of broker dependencies or exactly-once broker guarantees.
#endif
