# Technology Decisions

## .NET

Use .NET 10 LTS as the target framework for new generated projects.

## API

Use ASP.NET Core Minimal APIs by default.

## Database

Use PostgreSQL by default. Use EF Core with one DbContext per module and one schema per module.

## Aspire

Use Aspire in pro/advanced profiles for local orchestration, developer experience, service defaults, OpenTelemetry, and optional MCP-friendly runtime diagnostics.

## Observability

Use structured logging, health checks, OpenTelemetry traces/metrics, and clear correlation/request conventions.

## Mediator

Default to `core` internal dispatcher. Support MediatR as optional.

## Event sourcing

Do not use event sourcing by default. Support domain events, integration events, audit, and outbox-ready flows. Event sourcing requires an ADR.

## Guardrails runner

Use Node `.mjs` as the single cross-platform automation and guardrail runner. Do not duplicate the same logic in Bash and PowerShell.

## Hooks

Support Lefthook optionally. Hooks are local convenience; CI is enforcement.

## AI instructions

Use `AGENTS.md` as canonical. Tool-specific files only point to it.
