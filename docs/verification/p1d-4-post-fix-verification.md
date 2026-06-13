# P1D-4 Post-Fix Verification

Date: 2026-06-10

## Summary

P1D-4 deployment skeleton semantics are verified for generated `pro` and `advanced` outputs. No implementation fixes were made in this run, and no P2 public polish, UI, event sourcing, message broker dependency, public screenshots, badges, docs site, release polish, or project rename work was started.

Fresh validation evidence:

- First smoke run: `artifacts/template-smoke/runs/mq8aqf91-2c16410b`.
- Immediate second smoke run: `artifacts/template-smoke/runs/mq8b53vn-bbdd4132`.
- Primary generated evidence source: `artifacts/template-smoke/runs/mq8b53vn-bbdd4132`.

P1D-4 deployment behavior is confirmed closed, with one validation limitation: standalone `npm run check:security` takes a little over three minutes after fresh smoke artifacts accumulate, so shorter 120-second timeouts can fail even though the check completes successfully with a longer timeout.

## P1D-4 Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced container assets | Pass | Generated `pro-core` and `advanced-core` include `Dockerfile`, `.dockerignore`, `docker-compose.yml`, `.env.example`, and `src/<App>.Api/appsettings.Production.json`. |
| Core exclusion | Pass | Latest `core-core` output excludes `Dockerfile`, `.dockerignore`, `docker-compose.yml`, `.env.example`, and `appsettings.Production.json`; generated deployment docs explain the lightweight core profile. |
| Dockerfile path accuracy | Pass | Generated Dockerfiles reference concrete project paths such as `src/Smoke.ProCore.Api/Smoke.ProCore.Api.csproj` and `src/Smoke.ProCore.ServiceDefaults/Smoke.ProCore.ServiceDefaults.csproj`. |
| Dockerfile missing-file risk | Pass | Smoke restore/build/test passed for generated variants, and Dockerfile paths match generated projects in pro/advanced outputs. |
| Local compose scope | Pass | Generated compose starts only API and `postgres:17-alpine`, requires supplied local PostgreSQL/JWT values, and does not include broker, identity-provider, Kubernetes, registry, or cloud-provider infrastructure. |
| Configuration examples | Pass | `.env.example` and `appsettings.Production.json` cover PostgreSQL, JWT issuer/audience/signing key, logging, allowed hosts, inbox processor, resilience, and OpenTelemetry placeholders. |
| Secret hygiene | Pass | Production settings keep sensitive values empty; env/docs examples use obvious placeholders such as `replace-with-user-supplied-secret` and `replace-me`; no real registry/cloud credentials were found in deployment files. |
| CI/CD skeleton | Pass | Generated CI separates `dotnet` validation from `container` build and a `deployment-placeholder` job gated by manual dispatch plus `vars.ENABLE_DEPLOYMENT_PLACEHOLDER == 'true'`. |
| No default deployment | Pass | Generated CI does not push an image, log in to a registry, require deployment secrets, or deploy anywhere by default. |
| Health endpoint alignment | Pass | Dockerfile healthcheck and docs use `/health`; generated API maps `/health`; docs state it is a basic process health endpoint and does not require PostgreSQL by default. |
| Observability skeleton | Pass | Docs describe existing OpenTelemetry ASP.NET Core wiring and state no collector/backend is required by default. |
| Previous P1 behavior | Pass | Two smoke runs passed generated core/pro/advanced, core and MediatR variants, P1A guardrails/AI/docs/license semantics, P1B item templates, P1C architecture tests, P1D-1, P1D-2A, P1D-2B, P1D-3A, and P1D-3B checks. |
| Smoke quality | Pass | `assertP1D4DeploymentSkeletonSemantics` is present and invoked for generated main-template variants; two immediate smoke runs passed. |
| Security timeout | Pass with limitation | Full `npm run check` passed before and after smoke artifacts. Direct standalone `npm run check:security` also passed with a longer timeout, taking about 209 seconds with 66 smoke-run directories present. |

## Generated Output Evidence

Primary evidence came from `artifacts/template-smoke/runs/mq8b53vn-bbdd4132`.

Pro deployment assets:

- `g/pro-core/Dockerfile`
- `g/pro-core/.dockerignore`
- `g/pro-core/docker-compose.yml`
- `g/pro-core/.env.example`
- `g/pro-core/src/Smoke.ProCore.Api/appsettings.Production.json`
- `g/pro-core/.github/workflows/ci.yml`
- `g/pro-core/docs/deployment.md`
- `g/pro-core/docs/operations.md`

Advanced deployment assets:

- `g/advanced-core/Dockerfile`
- `g/advanced-core/.dockerignore`
- `g/advanced-core/docker-compose.yml`
- `g/advanced-core/.env.example`
- `g/advanced-core/src/Smoke.AdvancedCore.Api/appsettings.Production.json`
- `g/advanced-core/.github/workflows/ci.yml`
- `g/advanced-core/docs/deployment.md`

Core exclusion checks returned `False` for all of these paths in `g/core-core`: `Dockerfile`, `.dockerignore`, `docker-compose.yml`, `.env.example`, and `src/Smoke.CoreCore.Api/appsettings.Production.json`.

## Containerization Evidence

Generated pro Dockerfile evidence:

- copies `src/Smoke.ProCore.BuildingBlocks/Smoke.ProCore.BuildingBlocks.csproj`;
- copies `src/Smoke.ProCore.Modules/Smoke.ProCore.Modules.csproj`;
- copies `src/Smoke.ProCore.ServiceDefaults/Smoke.ProCore.ServiceDefaults.csproj`;
- copies `src/Smoke.ProCore.Api/Smoke.ProCore.Api.csproj`;
- runs `dotnet restore src/Smoke.ProCore.Api/Smoke.ProCore.Api.csproj`;
- publishes `src/Smoke.ProCore.Api/Smoke.ProCore.Api.csproj`;
- runs `Smoke.ProCore.Api.dll`;
- exposes `8080`;
- healthchecks `http://localhost:8080/health`.

Generated advanced Dockerfile has the same shape with `Smoke.AdvancedCore.*` project paths. The files use .NET 10 SDK/runtime images and do not reference missing `Aegis.Template` paths after generation.

Generated `.dockerignore` keeps local `.env` files out while preserving `.env.example`, and excludes build/test/docs/tool context such as `bin/`, `obj/`, `tests/`, `docs/`, `artifacts/`, and `tools/`.

Generated compose is local/dev oriented: API plus PostgreSQL only, with required `${POSTGRES_*:?Set ...}` and `${JWT_SIGNING_KEY:?Set JWT_SIGNING_KEY}` values.

## Config And Secrets Hygiene Evidence

Generated `appsettings.Production.json` keeps these sensitive placeholders empty:

- `ConnectionStrings:Postgres`
- `Authentication:Jwt:Issuer`
- `Authentication:Jwt:Audience`
- `Authentication:Jwt:SigningKey`
- `AllowedHosts`
- `OpenTelemetry:OtlpEndpoint`

It also includes safe defaults for logging, `Inbox:EnableBackgroundProcessor: false`, and `Resilience:DefaultTimeoutSeconds: 10`.

Generated `.env.example` includes placeholder values for database, JWT, logging, allowed hosts, OpenTelemetry, inbox, and resilience settings. The file explicitly says not to commit real passwords, JWT signing keys, registry tokens, or cloud credentials.

Root and generated deployment docs say production values must come from environment variables or the hosting platform's secret store. No cloud-specific secret manager dependency was introduced.

## CI And Deployment Placeholder Evidence

Generated `pro-core/.github/workflows/ci.yml` includes:

- `dotnet` job: restore, build, test, and generated guardrails;
- `container` job: `docker build --pull -t aegis-template-api:ci .`;
- `deployment-placeholder` job: depends on container, requires `workflow_dispatch`, requires `vars.ENABLE_DEPLOYMENT_PLACEHOLDER == 'true'`, and only prints guidance.

The workflow contains no registry login, image push, cloud provider CLI, Kubernetes deploy step, organization/repository/registry target, or deployment secret such as `REGISTRY_PASSWORD`, `DOCKER_PASSWORD`, `AZURE_CREDENTIALS`, `AWS_ACCESS_KEY_ID`, or `GCP_CREDENTIALS`.

Basic CI build/test behavior does not require deployment secrets.

## Health And Observability Evidence

The generated API maps `/health`, and the generated Dockerfile healthcheck calls that endpoint. Generated deployment docs state the default health endpoint is a basic process health endpoint and does not require PostgreSQL or an external service.

Generated docs describe structured logging settings and OpenTelemetry wiring as placeholders. They state no collector is required by default and that exporters can be wired later with environment variables or platform-specific hosting code.

## Smoke Assertions Reviewed

`tools/guardrails/check.mjs` includes `assertP1D4DeploymentSkeletonSemantics` and invokes it during generated variant validation.

Reviewed assertions cover:

- pro/advanced deployment asset presence;
- core deployment asset exclusion;
- Dockerfile generated API and ServiceDefaults project paths;
- Dockerfile runtime assembly and `/health` healthcheck;
- `.dockerignore` local-env and build-output behavior;
- `.env.example` placeholders for database, JWT, logging, allowed hosts, inbox, resilience, and observability;
- production settings empty placeholders;
- local/dev compose scope and no broker/Kubernetes claims;
- provider-neutral deployment docs;
- CI container build job;
- gated non-deploying deployment placeholder;
- absence of deployment secrets in CI;
- production project files not referencing Docker, compose, workflows, or env examples;
- no unresolved deployment template tokens.

Generated `DeploymentSkeletonTests` also assert profile behavior, deployment file secret/provider-target hygiene, and no production project references to deployment scripts or workflow files.

## Security Timeout Assessment

The earlier report noted standalone `npm run check:security` timed out once at 120 seconds, while full `npm run check` passed afterward.

This verification showed the security scan is slow after many smoke artifacts accumulate, but it does complete with an adequate timeout:

- `npm run check` before smoke: passed, including the security phase.
- `npm run check` after writing this report: passed in about 213 seconds, including the security phase.
- Direct `npm run check:security` after smoke artifacts accumulated: passed in about 209 seconds with a longer timeout.
- One PowerShell timing-wrapper attempt around `npm run check:security` timed out at 240 seconds, so that wrapper result is treated as timeout-limit evidence rather than a semantic failure.
- Current smoke-run directory count under `artifacts/template-smoke/runs`: 66.

The security check implementation walks the whole workspace with `listFiles(".")` to find sensitive-looking files and duplicate shell guardrail logic. With many generated smoke artifacts retained, that scan is too slow for standalone validation timeouts.

Assessment: P1D-4 deployment skeleton remains confirmed closed because deployment semantics, generated outputs, docs, CI placeholders, smoke assertions, full `npm run check`, and direct standalone security validation passed. The earlier 120-second timeout is acceptable as a documented timeout/performance limitation, not evidence of a failing guardrail. A separate cleanup can still reduce scan cost by pruning old smoke artifacts or teaching the security scan to skip generated artifact trees.

## Documentation And Acceptance

Verified root docs:

- `docs/deployment.md` says P1D-4 is scaffolding, not full production infrastructure.
- It explains image build/run, local compose, environment variables, secret handling, CI/deployment placeholders, `/health`, OpenTelemetry exporter wiring, no default cloud provider, and no default collector.
- `docs/testing.md` says smoke asserts P1D-4 semantics without Docker, Kubernetes, cloud credentials, registry credentials, live PostgreSQL, or deployment secrets.
- `docs/acceptance-criteria.md` marks P1D-4 complete with scoped criteria.
- `specs/0001-aegis-template-core/acceptance.md` marks P1D-4 complete with matching criteria.
- `OpenQuestions.md` contains `Q-20260610-002` and accurately states the provider-neutral, non-deploying deployment skeleton default.

`docs/operations.md` is not present at the repository root. Generated `docs/operations.md` is present in full-docs generated output and links to generated deployment guidance.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass before smoke; security phase included. |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq8aqf91-2c16410b`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq8b53vn-bbdd4132`. |
| `npm run check:security` through PowerShell timing wrapper | Timed out after 240 seconds after smoke artifacts accumulated; treated as timeout-limit evidence. |
| `npm run check` after writing this report | Pass; completed in about 213 seconds and included the security phase. |
| Direct `npm run check:security` after final check | Pass; completed in about 209 seconds. |
| Repeated `npm run check` after final report updates | Pass; repeated post-report runs completed in about 206-208 seconds and included the security phase. |

Generated tests observed in smoke included architecture, integration, contract, and performance smoke projects. Docker-backed Testcontainers tests remained skipped by default unless `AEGIS_RUN_TESTCONTAINERS=true` is set.

## Validation Limitations

Docker is unavailable on `PATH`, so the optional generated container image build smoke was not run. This is not a default validation failure because P1D-4 default validation intentionally must not require Docker.

No Kubernetes cluster, cloud credentials, registry credentials, broker, external identity provider, real JWT issuer, live database, external service, deployment secret, or production deployment target was used.

Standalone security scanning currently has a performance limitation when many smoke artifacts are retained; use a timeout above 120 seconds until that scan is optimized.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

Open blockers from `OpenQuestions.md`: none.

Relevant inferred decisions:

- `Q-20260610-002`: keep deployment skeleton provider-neutral and non-deploying by default.
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
- public polish and release presentation;
- real cloud deployment modules;
- registry push/publish automation;
- Kubernetes manifests;
- production secret-manager integration;
- external observability backend wiring.

## Closure

P1D-4 remains confirmed closed.

Recommended next goal: address the standalone security guardrail performance limitation before starting P2 public polish.
