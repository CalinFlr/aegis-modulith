# Aegis.Modulith

`Aegis.Modulith` is a pragmatic .NET modular monolith template system for API-only applications. It gives teams a buildable `dotnet new` starter with CQRS-lite, vertical slices, PostgreSQL, optional MediatR, Aspire-ready pro scaffolding, module manifests, spec-driven workflows, and AI development guardrails.

## Install

From a packed local build:

```bash
dotnet pack templates/Aegis.Modulith.Templates/Aegis.Modulith.Templates.csproj -c Release -o artifacts/packages
dotnet new install artifacts/packages/Aegis.Modulith.Templates.0.1.0-alpha.1.nupkg --force
```

After publish:

```bash
dotnet new install Aegis.Modulith.Templates
```

## Golden Path

Create the recommended pro profile with the core dispatcher:

```bash
dotnet new aegis-modulith -n Acme.WorkHub --profile pro --mediator core
cd Acme.WorkHub
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

Add a module and slices:

```bash
dotnet new aegis-module -n Billing --schema billing
dotnet new aegis-slice -n CreateInvoice --module Billing --kind command
dotnet new aegis-slice -n GetInvoiceById --module Billing --kind query
dotnet new aegis-event -n InvoiceIssued --module Billing --scope integration
dotnet new aegis-worker -n BillingOutboxDispatcher --module Billing
```

Generate the TaskHub reference sample:

```bash
dotnet new aegis-modulith -n Aegis.TaskHub --profile pro --sample taskhub
```

## Profiles

| Profile | Purpose | Included shape |
| --- | --- | --- |
| `core` | Minimal production-minded API foundation | Minimal APIs, CQRS-lite, vertical slices, PostgreSQL, OpenAPI, health checks, architecture tests |
| `pro` | Recommended default | Core plus Aspire-ready AppHost/ServiceDefaults, outbox, audit, idempotency, background worker, Dockerfile, CI-ready guardrails |
| `advanced` | Enterprise scaffolding | Pro plus permission and tenancy placeholders, deployment notes, strict guardrail stance, and contract-test skeletons |

Defaults:

```text
profile=pro
mediator=core
database=postgres
sample=none
ai=enterprise
guardrails=standard
hooks=none
skills=enterprise
docs=full
license=apache2
```

## Template Smoke Matrix

The repository smoke test validates:

| Variant | Command shape |
| --- | --- |
| Core + core mediator | `--profile core --mediator core` |
| Core + MediatR | `--profile core --mediator mediatr` |
| Pro + core mediator | `--profile pro --mediator core` |
| Pro + MediatR | `--profile pro --mediator mediatr` |
| Advanced + core mediator | `--profile advanced --mediator core` |
| Advanced + MediatR | `--profile advanced --mediator mediatr` |
| TaskHub sample | `--profile pro --sample taskhub` |
| Strict enterprise | `--profile advanced --ai enterprise --guardrails strict --hooks lefthook` |

Run it with:

```bash
npm run template:smoke
```

## Architecture Guardrails

Generated apps follow these defaults:

- API-only first.
- Modular monolith by default.
- CQRS-lite, not event sourcing by default.
- Vertical slices inside modules.
- PostgreSQL with one database, one schema per module, and one DbContext per module.
- No cross-module foreign keys.
- No generic repository over EF Core.
- Core dispatcher by default; MediatR is optional and pinned to a permissive-license line.
- Generated modules include `module.json` manifests.

## Repository Validation

```bash
npm run check
npm run check:ai
npm run check:docs
npm run check:security
npm run check:specs
npm run template:smoke
```

Generated solutions must also pass:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Docs

- [Project brief](docs/project-brief.md)
- [Architecture](docs/architecture.md)
- [CLI template spec](docs/cli-template-spec.md)
- [Profile guide](docs/getting-started/which-profile.md)
- [Module manifest guide](docs/architecture/module-manifest.md)
- [Spec-driven development](docs/ai-development/spec-driven-development.md)
- [Open questions policy](docs/open-questions-policy.md)

## License

Apache-2.0. See [LICENSE](LICENSE).
