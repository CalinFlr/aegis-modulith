# P1B Post-Fix Verification

Date: 2026-06-09

## Summary

P1B is confirmed closed after the item template polish implementation.

This run verified only the P1B item-template scope. No fixes were implemented. P1C architecture-test expansion, P1D pro/advanced feature-depth work, UI, event sourcing, public screenshots, badges, docs site work, P2 public polish, and project renaming were not started.

Primary generated-output evidence came from the second immediate smoke run:

```text
artifacts/template-smoke/runs/mq6adl30-6255f98c
```

The preceding immediate smoke run also passed:

```text
artifacts/template-smoke/runs/mq6a6zaw-46e3122e
```

## P1B Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| `aegis-module` architecture integration | Pass | Generated `Billing` module includes `Billing.csproj`, `BillingModule : IAegisModule`, service registration, EF Core `BillingDbContext`, required folders, migrations placeholder, and `module.json`. |
| `aegis-slice` command slice | Pass | Generated `Features/CreateInvoice` command record, handler, endpoint, validator, and response use the generated CQRS abstractions and namespace shape. |
| `aegis-slice` query slice | Pass | Generated `Features/ListInvoices` query record, handler, endpoint, and paged response use the generated CQRS abstractions and paged query shape. |
| MediatR compatibility | Pass | `pro-mediatr` and `advanced-mediatr` generated slices implement common CQRS abstractions plus `MediatR.IRequest`/`IRequestHandler`; core variants do not reference MediatR. |
| `aegis-event` domain/integration distinction | Pass | Domain event generated under `Domain/Events` and derives from `DomainEvent`; integration event generated under `Contracts/IntegrationEvents` and derives from `IntegrationEvent`; file hashes differ. |
| `aegis-worker` usefulness/buildability | Pass | Worker output builds and includes `BackgroundService`, logging, cancellation-token usage, hosted-service registration, and Program registration. |
| Smoke quality | Pass | Smoke assertions validate item-template semantics across `core-core`, `pro-core`, `advanced-core`, `pro-mediatr`, and `advanced-mediatr`, plus worker output and unresolved-token scans. |

## Exact Generated-Output Evidence

Latest generated run:

```text
artifacts/template-smoke/runs/mq6adl30-6255f98c
```

### `aegis-module`

Representative output path:

```text
g/core-core/src/Smoke.CoreCore.Modules/Modules/Billing
```

Observed module files include:

- `Billing.csproj`
- `BillingModule.cs`
- `module.json`
- `Contracts/BillingContracts.cs`
- `Domain/BillingEntity.cs`
- `Features/.gitkeep`
- `Infrastructure/BillingDbContext.cs`
- `Infrastructure/BillingServiceCollectionExtensions.cs`
- `Infrastructure/Migrations/.gitkeep`

Folder evidence:

| Folder | Exists |
| --- | --- |
| `Features` | Yes |
| `Domain` | Yes |
| `Infrastructure` | Yes |
| `Contracts` | Yes |
| `Infrastructure/Migrations` | Yes |

Project and namespace evidence:

```text
Billing.csproj:6: <RootNamespace>Smoke.CoreCore.Modules.Modules.Billing</RootNamespace>
Billing.csproj:10: <ProjectReference Include="../../../Smoke.CoreCore.BuildingBlocks/Smoke.CoreCore.BuildingBlocks.csproj" />
BillingModule.cs:9: namespace Smoke.CoreCore.Modules.Modules.Billing;
BillingModule.cs:11: public sealed class BillingModule : IAegisModule
BillingModule.cs:15: public string Schema => BillingDbContext.Schema;
BillingModule.cs:19: services.AddBillingInfrastructure(configuration);
BillingModule.cs:24: endpoints.MapGroup("/billing").WithTags(Name);
```

DbContext and service registration evidence:

```text
BillingDbContext.cs:4: namespace Smoke.CoreCore.Modules.Modules.Billing.Infrastructure;
BillingDbContext.cs:6: public sealed class BillingDbContext(DbContextOptions<BillingDbContext> options) : DbContext(options)
BillingDbContext.cs:10: public DbSet<BillingEntity> BillingEntities => Set<BillingEntity>();
BillingDbContext.cs:14: modelBuilder.HasDefaultSchema(Schema);
BillingServiceCollectionExtensions.cs:14: options.UseNpgsql(configuration.GetConnectionString("Postgres") ?? DefaultConnectionString));
```

`module.json` evidence:

```json
{
  "name": "Billing",
  "schema": "billing",
  "type": "business-module",
  "owner": "core",
  "dependencies": [],
  "publicContracts": [],
  "features": [],
  "rules": {
    "allowCrossModuleDatabaseAccess": false,
    "allowInfrastructureReferences": false
  }
}
```

Build evidence from the latest smoke run:

| Variant | Module project exists | Built `Billing.dll` exists | Solution exists |
| --- | --- | --- | --- |
| `core-core` | Yes | Yes | Yes |
| `pro-core` | Yes | Yes | Yes |
| `advanced-core` | Yes | Yes | Yes |
| `pro-mediatr` | Yes | Yes | Yes |
| `advanced-mediatr` | Yes | Yes | Yes |

### `aegis-slice`

Command slice output path:

```text
g/core-core/src/Smoke.CoreCore.Modules/Modules/Billing/Features/CreateInvoice
```

Command evidence:

```text
CreateInvoiceCommand.cs:3: namespace Smoke.CoreCore.Modules.Modules.Billing.Features.CreateInvoice;
CreateInvoiceCommand.cs:5: public sealed record CreateInvoiceCommand(string Name) : ICommand<CreateInvoiceResponse>
CreateInvoiceCommandHandler.cs:6: ICommandHandler<CreateInvoiceCommand, CreateInvoiceResponse>
CreateInvoiceCommandEndpoint.cs:14: ICommandDispatcher dispatcher,
CreateInvoiceCommandValidator.cs:5: public sealed class CreateInvoiceCommandValidator : IValidator<CreateInvoiceCommand>
CreateInvoiceResponse.cs:3: public sealed record CreateInvoiceResponse(Guid Id, string Name, DateTimeOffset CreatedAtUtc);
```

Query slice output path:

```text
g/core-core/src/Smoke.CoreCore.Modules/Modules/Billing/Features/ListInvoices
```

Query evidence:

```text
ListInvoicesQuery.cs:3: namespace Smoke.CoreCore.Modules.Modules.Billing.Features.ListInvoices;
ListInvoicesQuery.cs:5: public sealed record ListInvoicesQuery(int PageNumber = 1, int PageSize = 50) : IQuery<ListInvoicesResponse>
ListInvoicesQueryEndpoint.cs:13: [AsParameters] ListInvoicesQuery query,
ListInvoicesQueryEndpoint.cs:14: IQueryDispatcher dispatcher,
ListInvoicesQueryHandler.cs:6: IQueryHandler<ListInvoicesQuery, ListInvoicesResponse>
ListInvoicesQueryHandler.cs:11: var pageNumber = Math.Max(1, query.PageNumber);
ListInvoicesQueryHandler.cs:12: var pageSize = Math.Clamp(query.PageSize, 1, 100);
ListInvoicesResponse.cs:6: IReadOnlyList<ListInvoicesItemResponse> Items,
ListInvoicesResponse.cs:7: int PageNumber,
ListInvoicesResponse.cs:8: int PageSize,
```

### MediatR Compatibility

The generated slice outputs keep the common CQRS contracts and add MediatR interfaces when `--mediator mediatr` is used.

`pro-mediatr` evidence:

```text
CreateInvoiceCommand.cs:5: public sealed record CreateInvoiceCommand(string Name) : ICommand<CreateInvoiceResponse>
CreateInvoiceCommand.cs:6: , MediatR.IRequest<CreateInvoiceResponse>
CreateInvoiceCommandHandler.cs:7: , MediatR.IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>
CreateInvoiceCommandHandler.cs:16: Task<CreateInvoiceResponse> MediatR.IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>.Handle(
ListInvoicesQuery.cs:5: public sealed record ListInvoicesQuery(int PageNumber = 1, int PageSize = 50) : IQuery<ListInvoicesResponse>
ListInvoicesQuery.cs:6: , MediatR.IRequest<ListInvoicesResponse>
ListInvoicesQueryHandler.cs:7: , MediatR.IRequestHandler<ListInvoicesQuery, ListInvoicesResponse>
ListInvoicesQueryHandler.cs:18: Task<ListInvoicesResponse> MediatR.IRequestHandler<ListInvoicesQuery, ListInvoicesResponse>.Handle(
```

`advanced-mediatr` evidence matches the same compatibility pattern. Smoke also asserts that core variants do not reference `MediatR.IRequest` or `MediatR.IRequestHandler`.

### `aegis-event`

Domain event output:

```text
g/core-core/src/Smoke.CoreCore.Modules/Modules/Billing/Domain/Events/InvoiceIssuedDomainEvent.cs
```

Integration event output:

```text
g/core-core/src/Smoke.CoreCore.Modules/Modules/Billing/Contracts/IntegrationEvents/InvoiceIssuedIntegrationEvent.cs
```

Event evidence:

```text
InvoiceIssuedDomainEvent.cs:3: namespace Smoke.CoreCore.Modules.Modules.Billing.Domain.Events;
InvoiceIssuedDomainEvent.cs:6: : DomainEvent(OccurredAtUtc);
InvoiceIssuedIntegrationEvent.cs:3: namespace Smoke.CoreCore.Modules.Modules.Billing.Contracts.IntegrationEvents;
InvoiceIssuedIntegrationEvent.cs:9: : IntegrationEvent(Id, OccurredAtUtc);
```

Distinct-output evidence:

| File | SHA-256 |
| --- | --- |
| `Domain/Events/InvoiceIssuedDomainEvent.cs` | `825E6F594465AC89A4B2DEB1E53CC1BDD726A110C551FE7A088FE538ED42D816` |
| `Contracts/IntegrationEvents/InvoiceIssuedIntegrationEvent.cs` | `6B7E73158217C987013ED1898C45B5F14B602F6D561DE198CAE7D1FB9C54DC79` |

### `aegis-worker`

Worker output path:

```text
i/worker
```

Worker files include:

- `BillingOutboxDispatcher.cs`
- `BillingOutboxDispatcher.csproj`
- `BillingOutboxDispatcherServiceCollectionExtensions.cs`
- `Directory.Packages.props`
- `Program.cs`

Worker evidence:

```text
BillingOutboxDispatcher.cs:6: public sealed class BillingOutboxDispatcher(ILogger<BillingOutboxDispatcher> logger) : BackgroundService
BillingOutboxDispatcher.cs:8: protected override async Task ExecuteAsync(CancellationToken stoppingToken)
BillingOutboxDispatcher.cs:10: logger.LogInformation("{WorkerName} for module {ModuleName} started.", nameof(BillingOutboxDispatcher), "Billing");
BillingOutboxDispatcher.cs:12: while (!stoppingToken.IsCancellationRequested)
BillingOutboxDispatcher.cs:15: await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
BillingOutboxDispatcher.cs:21: logger.LogDebug("{WorkerName} for module {ModuleName} completed one background pass.", nameof(BillingOutboxDispatcher), "Billing");
BillingOutboxDispatcherServiceCollectionExtensions.cs:9: services.AddHostedService<BillingOutboxDispatcher>();
Program.cs:4: builder.Services.AddBillingOutboxDispatcher();
```

Worker build evidence:

```text
BillingOutboxDispatcher -> artifacts/template-smoke/runs/mq6adl30-6255f98c/i/worker/bin/Release/net10.0/BillingOutboxDispatcher.dll
Build succeeded.
0 Warning(s)
0 Error(s)
```

### Template Token Scan

Targeted scan over representative latest item outputs found:

```text
No unresolved template tokens found in sampled latest item outputs.
```

The smoke runner also applies `assertNoTemplateTokens` to every item output it generates.

## Smoke Assertions Reviewed

Reviewed `tools/guardrails/check.mjs` item-template assertions:

- `assertNoTemplateTokens`
- `assertItemModuleSemantics`
- `assertItemSliceSemantics`
- `assertItemEventSemantics`
- `assertItemWorkerSemantics`

Reviewed smoke generation/build flow:

- Generates `aegis-module`, command `aegis-slice`, paged query `aegis-slice`, domain `aegis-event`, and integration `aegis-event` into generated module projects.
- Builds generated `Billing.csproj`.
- Builds the generated solution after item template generation.
- Runs item-template compatibility for `core-core`, `pro-core`, `advanced-core`, `pro-mediatr`, and `advanced-mediatr`.
- Generates and builds `aegis-worker` output.

Assertion coverage includes:

- module project file, generated namespace, BuildingBlocks project reference, `IAegisModule`, service registration, route group, EF Core `DbContext`, `DbSet`, schema configuration, PostgreSQL registration, required manifest fields, and manifest boundary rules;
- command/query slice folder shape, namespaces, CQRS command/query abstractions, command/query handler abstractions, endpoint dispatchers, command validator, response records, paged query binding, and paged response shape;
- MediatR additions for `--mediator mediatr` and negative MediatR assertions for core mode;
- domain event path/namespace/`DomainEvent` abstraction and integration event path/namespace/`IntegrationEvent` abstraction;
- worker `BackgroundService`, logging, cancellation-token observation and propagation, hosted-service registration, and Program registration;
- unresolved template token checks for `.cs`, `.csproj`, `.json`, `.props`, and `.md` item outputs.

## Checks Run

| Command | Result | Evidence |
| --- | --- | --- |
| `npm run check` | Pass | `ai instructions`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, and `security` passed. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq6a6zaw-46e3122e`. |
| `npm run template:smoke` again immediately | Pass | Run directory: `artifacts/template-smoke/runs/mq6adl30-6255f98c`. |

The smoke runs packed and installed `Aegis.Modulith.Templates`, generated the full smoke matrix, restored/built/tested generated solutions, ran generated guardrails where applicable, generated item templates into compatible generated outputs, built item outputs, and completed with `template smoke` passing.

## P1B Closure

P1B is confirmed closed.

The item templates are architecture-integrated drop-in outputs for generated Aegis.Modulith solutions, and smoke validation now proves their semantics rather than only checking file existence.

## Remaining P1C/P1D/P2 Gaps

The following remain intentionally outside this verification:

- P1C architecture-test expansion, including deeper automated enforcement for query non-mutation, DTO/entity separation, no cross-module EF navigation properties, and broader module boundary rules.
- P1D pro/advanced feature-depth work beyond the current validated scaffolding.
- P2 public polish such as public screenshots, badges, docs site, release presentation, and broader public-facing polish.
- UI work remains out of scope.
- Event sourcing remains out of scope and was not introduced.
- Project rename work remains out of scope and was not started.

## Validation Limitations

- Verification was performed on Windows only. The smoke runner is Node-based and avoids shell-specific smoke logic, but this run did not execute on Linux or macOS.
- The smoke matrix builds and tests generated solutions and item outputs, but it does not run generated APIs and exercise HTTP endpoints.
- Item templates are validated as drop-in generated outputs. They do not automatically mutate `.sln`, project references, or `Program.cs`; this follows the existing inferred decision in `OpenQuestions.md` (`Q-20260609-001`).
- The requested read paths `docs/architecture/vertical-slices.md` and `docs/architecture/cqrs-lite.md` are not present. Verification used the canonical rules in `docs/architecture.md`, `docs/cli-template-spec.md`, and the core spec/acceptance files instead.
- This run did not manually inspect every generated file in every variant; it combined static review of template sources, semantic smoke assertions, and targeted generated-output inspection from the latest smoke run.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

No new true blockers or inferred decisions were identified.

Existing open or inferred decisions remain:

- `Q-20260606-001`: Confirm final public project name before publish.
- `Q-20260606-002`: Adopt spec-driven development layer.
- `Q-20260606-003`: Include `module.json` manifests in generated modules.
- `Q-20260606-004`: Pin optional MediatR to Apache-2.0 package line.
- `Q-20260608-001`: Define the generated core AI skill subset.
- `Q-20260609-001`: Prefer drop-in item templates over automatic solution mutation.

No blockers are listed in `OpenQuestions.md`.

## Spec Folder

Used spec folder:

```text
specs/0001-aegis-template-core
```

No spec files were updated.

## Recommended Next Step

Start P1C architecture-test expansion as a separate goal.
