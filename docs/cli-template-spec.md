# CLI Template Specification

## Template package

Package name:

```text
Aegis.Modulith.Templates
```

Install:

```bash
dotnet new install Aegis.Modulith.Templates
```

## Main template

```bash
dotnet new aegis-modulith -n Acme.WorkHub
```

Equivalent default:

```bash
dotnet new aegis-modulith \
  -n Acme.WorkHub \
  --profile pro \
  --mediator core \
  --database postgres \
  --sample none \
  --ai enterprise \
  --guardrails standard \
  --hooks none \
  --skills enterprise \
  --docs full \
  --license apache2
```

## Main options

```text
--profile core|pro|advanced
--mediator core|mediatr
--database postgres
--sample none|taskhub
--ai none|agents|enterprise
--guardrails off|standard|strict
--hooks none|lefthook
--skills none|core|enterprise
--docs standard|full
--license apache2|mit
```

## Profile: core

Minimal production-minded API foundation:

- .NET 10.
- ASP.NET Core Minimal APIs.
- Modular monolith skeleton.
- CQRS-lite.
- Vertical slices.
- Core dispatcher.
- PostgreSQL.
- EF Core.
- OpenAPI.
- ProblemDetails.
- Result pattern.
- Validation abstraction.
- Basic health checks.
- Basic logging/OpenTelemetry wiring.
- Architecture tests.
- Integration test skeleton.
- AGENTS.md if AI enabled.

## Profile: pro

Recommended default:

- Everything in core.
- Aspire AppHost and ServiceDefaults.
- Outbox skeleton.
- Inbox pattern.
- Background worker skeleton.
- Audit log.
- Idempotency support.
- Rate limiting baseline.
- Caching abstraction.
- HttpClient resilience defaults.
- Testcontainers.
- Fake auth test handler.
- Dockerfile.
- Node `.mjs` guardrail runner.
- AI workflows, policies, skills, docs.
- GitHub Actions.

## Profile: advanced

Enterprise scaffolding:

- Everything in pro.
- Permission model.
- JWT auth scaffolding.
- Multi-tenancy skeleton.
- Optional broker-ready structure.
- Inbox pattern.
- Contract tests.
- Performance smoke tests.
- Deployment skeleton.
- Strict guardrails.
- Optional Lefthook.
- MCP-ready documentation.

## Item templates

### Module

```bash
dotnet new aegis-module \
  -n Billing \
  --schema billing \
  --rootNamespace Acme.WorkHub.Modules \
  --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks \
  --buildingBlocksProject ../../../Acme.WorkHub.BuildingBlocks/Acme.WorkHub.BuildingBlocks.csproj \
  -o src/Acme.WorkHub.Modules/Modules/Billing
```

### Command slice

```bash
dotnet new aegis-slice \
  -n CreateInvoice \
  --module Billing \
  --kind command \
  --mediator core \
  --rootNamespace Acme.WorkHub.Modules \
  --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks \
  -o src/Acme.WorkHub.Modules/Modules/Billing
```

### Query slice

```bash
dotnet new aegis-slice \
  -n ListInvoices \
  --module Billing \
  --kind query \
  --paged true \
  --mediator core \
  --rootNamespace Acme.WorkHub.Modules \
  --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks \
  -o src/Acme.WorkHub.Modules/Modules/Billing
```

Use `--mediator mediatr` for slices added to generated MediatR solutions.

### Event

```bash
dotnet new aegis-event \
  -n InvoiceIssued \
  --module Billing \
  --scope domain \
  --rootNamespace Acme.WorkHub.Modules \
  --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks \
  -o src/Acme.WorkHub.Modules/Modules/Billing

dotnet new aegis-event \
  -n InvoiceIssued \
  --module Billing \
  --scope integration \
  --rootNamespace Acme.WorkHub.Modules \
  --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks \
  -o src/Acme.WorkHub.Modules/Modules/Billing
```

### Worker

```bash
dotnet new aegis-worker -n BillingOutboxDispatcher --module Billing
```

## Smoke matrix

CI must generate and validate at least:

```text
core + mediator core
core + mediator mediatr
pro + mediator core
pro + mediator mediatr
advanced + mediator core
advanced + mediator mediatr
pro + sample taskhub
advanced + ai enterprise + strict guardrails + lefthook
```
