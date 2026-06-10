# P1D-4 Deployment Skeleton Verification

Date: 2026-06-10

## Summary

P1D-4 adds provider-neutral deployment scaffolding for generated `pro` and `advanced` API outputs.

This implementation did not add UI, event sourcing, a message broker dependency, public screenshots, badges, a docs site, P2 public polish, a project rename, or architecture redesign.

Fresh validation evidence:

- Initial implementation smoke: `artifacts/template-smoke/runs/mq80044d-8c93b30f`.
- Required first smoke run: `artifacts/template-smoke/runs/mq80d71m-96b97ec3`.
- Required immediate second smoke run: `artifacts/template-smoke/runs/mq80mvwi-ed270397`.
- Primary generated evidence source: `artifacts/template-smoke/runs/mq80mvwi-ed270397`.

## P1D-4 Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced Dockerfile | Pass | Generated pro and advanced outputs include a multi-stage `Dockerfile` that restores and publishes `src/<App>.Api/<App>.Api.csproj`, uses .NET 10 SDK/runtime images, runs `<App>.Api.dll`, exposes port `8080`, and declares a `/health` healthcheck. |
| Core exclusion | Pass | Generated core outputs exclude `Dockerfile`, `.dockerignore`, `docker-compose.yml`, `.env.example`, and `src/<App>.Api/appsettings.Production.json`; generated `docs/deployment.md` explains the exclusion. |
| Dockerfile path accuracy | Pass | Smoke asserts generated Dockerfile paths contain the concrete generated API and ServiceDefaults project names, not `Aegis.Template` tokens. |
| `.dockerignore` | Pass | Generated pro/advanced outputs include `.dockerignore` excluding build outputs, test/docs/tool context, local `.env` files, and preserving `.env.example`. |
| Local compose | Pass | Generated `docker-compose.yml` starts only API plus `postgres:17-alpine`, requires caller-supplied local PostgreSQL/JWT values, and does not include broker, identity-provider, Kubernetes, registry, or cloud-provider infrastructure. |
| Environment example | Pass | `.env.example` includes placeholders for `ASPNETCORE_ENVIRONMENT`, PostgreSQL connection string, JWT issuer/audience/signing key, logging level, allowed hosts, OpenTelemetry endpoint, inbox processor, and resilience settings. |
| Production config sample | Pass | `appsettings.Production.json` uses empty placeholders for database, JWT, allowed hosts, inbox, resilience, and OpenTelemetry settings; missing JWT settings preserve reject-all auth behavior. |
| Secret hygiene | Pass | Smoke and generated architecture tests assert deployment files avoid hardcoded real secrets and common provider/registry targets. |
| CI validation separation | Pass | Generated pro/advanced CI keeps the `dotnet` validation job separate from a `container` image build job. Basic build/test does not require deployment secrets. |
| Deployment placeholder | Pass | Generated `deployment-placeholder` is manual and gated by `vars.ENABLE_DEPLOYMENT_PLACEHOLDER == 'true'`; it prints guidance and does not deploy anywhere by default. |
| Health endpoint alignment | Pass | Generated API already maps `/health`; deployment docs and Dockerfile healthcheck use that endpoint. |
| Observability guidance | Pass | Generated docs explain existing OpenTelemetry wiring and that no collector/exporter backend is required by default. |
| Architecture boundaries | Pass | Generated `DeploymentSkeletonTests` assert production projects do not reference Docker, compose, `.env.example`, or workflow files. |
| Smoke assertions | Pass | `tools/guardrails/check.mjs` includes `assertP1D4DeploymentSkeletonSemantics` and invokes it for every generated main-template variant. |
| Default runtime independence | Pass | Default validation did not require Docker, Kubernetes, cloud credentials, registry credentials, live PostgreSQL, external services, or deployment secrets. |
| Prior behavior intact | Pass | Two full smoke runs passed generated core/pro/advanced, core and MediatR variants, TaskHub, P1A AI/guardrail/docs/license semantics, P1B item templates, P1C architecture tests, P1D-1, P1D-2A, P1D-2B, P1D-3A, and P1D-3B checks. |

## Smoke Assertions Added

`assertP1D4DeploymentSkeletonSemantics` verifies:

- pro/advanced include `Dockerfile`, `.dockerignore`, `docker-compose.yml`, `.env.example`, and `appsettings.Production.json`;
- core excludes those assets;
- Dockerfile references generated project paths and `/health`;
- env/config examples contain placeholders for database, JWT, logging, allowed hosts, inbox, resilience, and observability;
- compose is local/dev API plus PostgreSQL only;
- CI container build does not require deployment secrets;
- deployment placeholder is gated and non-deploying by default;
- generated docs explain exclusions, health, observability, and provider neutrality;
- production projects do not reference deployment scripts;
- generated deployment files contain no unresolved template tokens.

Generated `DeploymentSkeletonTests` repeats profile behavior, secret/provider-target hygiene, and production-project boundary checks inside every generated solution.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check:docs` | Pass. |
| `npm run check:specs` | Pass. |
| `npm run check:security` with 120s timeout | Timed out during workspace scan before final validation; rerun through `npm run check` passed. |
| `npm run template:smoke` during implementation | Pass; run directory `artifacts/template-smoke/runs/mq80044d-8c93b30f`. |
| `npm run check` | Pass. |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq80d71m-96b97ec3`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq80mvwi-ed270397`. |
| `npm run check` after writing this report | Pass. |

Generated pro/advanced integration tests continued to skip Docker-backed Testcontainers tests by default unless `AEGIS_RUN_TESTCONTAINERS=true` is set.

## Docker Limitation

`Get-Command docker -ErrorAction SilentlyContinue` returned no Docker executable on `PATH`, so the optional live container image build was not run.

This is not a default validation failure. P1D-4 default validation is intentionally semantic and must not require Docker.

## OpenQuestions.md Updates

`OpenQuestions.md` now includes:

- `Q-20260610-002`: keep deployment skeleton provider-neutral and non-deploying by default.

Open blockers from `OpenQuestions.md`: none.

Relevant inferred decisions:

- `Q-20260610-002`: provider-neutral deployment skeleton, no default deploy.
- `Q-20260610-001`: diagnostic performance smoke tests for pro/advanced only.
- `Q-20260609-010`: contract tests for pro/advanced only.
- `Q-20260609-004`: JWT bearer plus claim-policy scaffold defaults.
- `Q-20260609-005` through `Q-20260609-007`: inbox idempotency defaults and core exclusion.
- `Q-20260609-002`: Docker-backed Testcontainers tests remain opt-in.

## Remaining P2 Gaps

The following remain outside P1D-4:

- public screenshots;
- badges;
- docs site;
- public polish/release presentation;
- real cloud deployment modules;
- registry push/publish automation;
- Kubernetes manifests;
- production secret-manager integration;
- external observability backend wiring.

## Closure

P1D-4 is confirmed closed.
