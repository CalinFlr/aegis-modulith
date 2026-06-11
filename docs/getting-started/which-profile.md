# Which Profile Should I Use?

Aegis.Modulith provides three profiles. Profiles are high-level defaults. Individual options can still override them.

## Recommendation

Use `pro` by default.

```bash
dotnet new aegis-modulith -n Acme.WorkHub --profile pro
```

`pro` is the best daily-driver profile because it gives you a practical API foundation without jumping straight to advanced enterprise complexity.

## Core

Use `core` when you want the smallest production-minded API foundation.

Best for:

- Personal APIs.
- Experiments.
- Small internal tools.
- Learning the architecture.

Includes:

- API-only ASP.NET Core.
- Modular monolith skeleton.
- CQRS-lite.
- Vertical slices.
- PostgreSQL structure.
- OpenAPI.
- ProblemDetails.
- Basic tests and architecture rules.

Avoid if:

- You need outbox, audit, Aspire, richer observability, or enterprise AI guardrails from day one.

## Pro

Use `pro` for most real projects.

Best for:

- New APIs you expect to maintain.
- SaaS backends.
- Team projects.
- Open-source examples.

Includes everything in `core`, plus:

- Aspire AppHost and ServiceDefaults.
- Outbox skeleton.
- Audit skeleton.
- Idempotency support.
- Testcontainers integration test setup.
- Dockerfile.
- Stronger OpenTelemetry setup.
- Enterprise AI assets.
- Node `.mjs` guardrails.

## Advanced

Use `advanced` when you want a stronger enterprise scaffold.

Best for:

- Larger teams.
- Security-conscious environments.
- Strict AI-assisted development.
- Projects that need tenancy, broker, deployment, or stronger policies.

Includes everything in `pro`, plus optional advanced scaffolding for:

- Tenancy.
- Permission/auth skeleton.
- Broker integration.
- Inbox pattern.
- Contract tests.
- Deployment skeleton.
- Strict guardrails.
- Lefthook hooks.

## Mediator choice

Default:

```bash
--mediator core
```

Use core when you want low magic, no MediatR dependency, and simpler AI reasoning.

Optional:

```bash
--mediator mediatr
```

Use MediatR only when your team explicitly wants that ecosystem and accepts the dependency and licensing implications.

## AI guardrails

Recommended:

```bash
--ai enterprise --guardrails standard
```

Use strict guardrails when humans want approval gates for dependencies, auth, migrations, public API changes, CI changes, and release operations.
