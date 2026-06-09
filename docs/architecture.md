# Architecture

## Architectural style

`Aegis.Modulith` uses a modular monolith with CQRS-lite and vertical slices.

Focused rule docs:

- [CQRS-lite](architecture/cqrs-lite.md)
- [Vertical slices](architecture/vertical-slices.md)
- [Module manifests](architecture/module-manifest.md)
- [Authentication and permissions](authentication.md)
- [Messaging and inbox](messaging.md)
- [Contracts and contract tests](contracts.md)

## Modular monolith

The application is deployed as one process by default, but internally divided into business modules. Modules own their own features, domain model, infrastructure, persistence schema, contracts, and tests.

Rules:

- A module owns its schema.
- A module owns its DbContext.
- A module owns its migrations.
- A module owns its contracts.
- No module may reference another module's Infrastructure.
- No cross-module foreign keys.
- Cross-module communication uses contracts, integration events, or documented application services.

## CQRS-lite

CQRS is applied at the use-case level:

- Commands change state.
- Queries read state.
- Commands and queries have separate request/response models.
- Commands use domain behavior when business rules justify it.
- Queries use read projections and `AsNoTracking` where EF Core is used.
- Same PostgreSQL database by default.
- No separate read/write database by default.

## Vertical slices

Features are grouped by use case inside modules:

```text
modules/
  Projects/
    Aegis.TaskHub.Projects/
      Features/
        CreateProject/
          CreateProjectEndpoint.cs
          CreateProjectCommand.cs
          CreateProjectHandler.cs
          CreateProjectValidator.cs
          CreateProjectResponse.cs
        GetProjectById/
          GetProjectByIdEndpoint.cs
          GetProjectByIdQuery.cs
          GetProjectByIdHandler.cs
          GetProjectByIdResponse.cs
```

## Event sourcing decision

Event sourcing is not used by default. The template supports domain events, integration events, and outbox-ready flows, but the database stores current state as the primary source of truth.

Event sourcing may be introduced later for selected modules such as ledger, inventory movement history, or audit-critical workflows, but only with an ADR.

The inbox pattern is a state-based idempotency scaffold for inbound integration messages in generated `pro` and `advanced` profiles. It is not event sourcing and does not add a broker dependency.

## Mediator decision

The default mediator is `core`, a lightweight internal command/query dispatcher. MediatR is optional through `--mediator mediatr`.

## Database decision

Default database is PostgreSQL:

- One database.
- Schema per module.
- DbContext per module.
- Migrations per module.
- No cross-module FK constraints.

## Enterprise AI development system

The repository includes a vendor-neutral AI development system:

- `AGENTS.md` canonical instructions.
- `.agents/skills` for Codex skills.
- `.ai/workflows` for end-to-end workflows.
- `.ai/policies` for risk/security/dependency rules.
- `.ai/guardrails` for deterministic checks and rules.
- `tools/guardrails/check.mjs` as Node runner.
- CI as final authority.
