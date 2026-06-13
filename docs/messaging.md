# Messaging

P1D-2B adds an inbox pattern scaffold for idempotent inbound integration event and message processing in generated `pro` and `advanced` profiles.

The inbox is a database-backed idempotency foundation. It is not event sourcing, and it does not include a broker by default.

## Purpose

Use the inbox when an external broker, webhook, scheduled importer, or another inbound consumer delivers an integration event to the API. The consumer records the inbound message first, then processing happens from the persisted inbox row.

This gives the application a transparent state record for first acceptance, duplicate detection by stable `MessageId` and `IdempotencyKey`, processing state, retry attempts, failure reason, and processed timestamp.

## Generated Shape

Generated `pro` and `advanced` outputs include `InboxMessage`, `AegisInboxDbContext`, EF Core configuration for `integration.inbox_messages`, unique indexes on `MessageId` and `IdempotencyKey`, `IInboxStore`, `EfCoreInboxStore`, `InboxProcessor`, `IInboxMessageHandler`, an opt-in `InboxProcessorWorker`, a sample integration-event handler, and fast generated tests.

The default strategy is simple database uniqueness. A message is accepted once, duplicates are ignored or reported, and already processed messages are not processed a second time.

## Processor Execution

No broker is included by default.

A real broker, webhook, queue consumer, or import endpoint should call `IInboxStore.AcceptAsync(...)` with the inbound message id, message type, and serialized payload. The scaffolded processor can then dispatch pending rows to registered handlers.

The background processor is opt-in:

```json
{
  "Inbox": {
    "EnableBackgroundProcessor": true
  }
}
```

Leave it disabled until the application has a real database migration and an explicit inbound consumer path.

## Retry And Failure State

Processing changes a message from `Pending` or `Failed` to `Processing`, increments `AttemptCount`, and applies a short lease marker. Successful handling marks the row `Processed` and records `ProcessedAtUtc`.

Handler failures mark the row `Failed` and store `FailureReason`. The simple processor can pick up failed rows again on a later pass.

## Relation To Integration Events And Outbox

Integration event contracts live under module `Contracts` folders. Inbox handlers consume those contracts and must not deserialize domain entities as payloads.

P1D-3A adds a lightweight contract-testing convention for integration events: generated integration event records declare `IntegrationEventContractAttribute` with a stable type name and positive integer version. The generated inbox sample handler uses that metadata as its message type. This is contract metadata only; it is not event sourcing and it does not add a broker.

The outbox and inbox solve opposite sides of message flow: outbox records outbound integration messages before publishing, while inbox records inbound integration messages before handling. Both remain state-based database scaffolds. Neither adds event sourcing or a broker dependency.
