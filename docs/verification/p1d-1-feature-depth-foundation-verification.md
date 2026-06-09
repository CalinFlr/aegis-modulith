# P1D-1 Feature-Depth Foundation Verification

Date: 2026-06-09

## Summary

P1D-1 is confirmed closed for the generated pro and advanced feature-depth foundation.

Implemented scope:

- Testcontainers PostgreSQL integration test foundation for generated pro and advanced outputs.
- Fake authentication test infrastructure for generated pro and advanced integration tests.
- Microsoft-first HttpClient resilience defaults for generated pro and advanced APIs.
- Semantic smoke assertions proving P1D-1 file shape, package references, profile behavior, and fake-auth production isolation.

Out-of-scope items were not started: P1D-2 JWT/auth scaffolding, permissions, inbox pattern; P1D-3 contract tests or performance smoke tests; P1D-4 deployment skeleton; UI; event sourcing; public screenshots, badges, docs site, or P2 polish.

## P1D-1 Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Testcontainers foundation | Pass | Pro/advanced outputs include `tests/<App>.IntegrationTests`, `Testcontainers.PostgreSql`, `PostgresContainerFixture`, `AegisWebApplicationFactory`, and `ContainerizedPostgresSmokeTests`. |
| Default smoke does not require Docker | Pass | Docker-backed tests use `DockerFactAttribute` and are skipped unless `AEGIS_RUN_TESTCONTAINERS=true` is set. |
| Database configuration path | Pass | Integration factory overrides `ConnectionStrings:Postgres`; `DatabaseInitialization` documents where future `Database.MigrateAsync` calls belong when generated migrations exist. |
| Fake auth test foundation | Pass | Integration tests include `Aegis.Test` scheme, fake handler, test users, `X-Test-*` headers, authenticated client helpers, and a fake-auth smoke test. |
| Fake auth production isolation | Pass | Smoke asserts production `Program.cs` does not contain fake auth handler/defaults, `X-Test-User-Id`, or `Aegis.Test`. |
| HttpClient resilience defaults | Pass | Pro/advanced API projects reference `Microsoft.Extensions.Http.Resilience`; `AddAegisOutboundHttpClients` wires `ConfigureHttpClientDefaults` and `AddStandardResilienceHandler`. |
| Core profile exclusions | Pass | Smoke asserts core outputs omit integration test project, Testcontainers package versions, fake auth, pro HTTP resilience files, and resilience package references. |
| Mediator variants | Pass | Pro/advanced `core` and `mediatr` variants restore, build, and test with the new foundations. |
| Smoke idempotency | Pass | Two immediate `npm run template:smoke` runs passed in separate run directories. |

## Generated Output Evidence

Clean smoke run directories:

```text
artifacts/template-smoke/runs/mq6ixpi7-9eb685ea
artifacts/template-smoke/runs/mq6j8leb-35dd52f0
```

Representative pro generated output from the second run:

```text
g/pro-core/tests/Smoke.ProCore.IntegrationTests
g/pro-core/src/Smoke.ProCore.Api/Pro/Http
```

The generated integration test run reported:

```text
Passed: 1, Skipped: 1, Total: 2
```

The skipped test is `ContainerizedPostgresSmokeTests.Api_host_starts_against_containerized_postgres`, which is intentionally Docker-gated. The passing test is the fake-auth smoke test.

## Smoke Assertions Added

`tools/guardrails/check.mjs` now asserts that pro and advanced outputs include:

- Testcontainers PostgreSQL and WebApplicationFactory package references.
- Generated integration test project in the solution.
- PostgreSQL container fixture, Docker opt-in fact, connection-string override factory, database initialization placeholder, and API host smoke test.
- Fake authentication handler, defaults, headers, test users, authenticated client helpers, and fake-auth smoke test.
- No fake authentication wiring in production `Program.cs`.
- `Microsoft.Extensions.Http.Resilience`, resilient HTTP registration extension, `AddStandardResilienceHandler`, sample typed external client, and pro service wiring.

It also asserts that core outputs exclude these P1D-1 pro/advanced assets.

## Checks Run

| Command | Result | Evidence |
| --- | --- | --- |
| Targeted pro generation/build/test | Pass | `artifacts/p1d-targeted-b58da6d6`; pro output built with 0 warnings/errors, architecture tests passed 23, integration tests passed 1 and skipped 1 Docker test. |
| `npm run check` | Pass | Repository guardrails passed before required smoke runs. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq6ixpi7-9eb685ea`. |
| `npm run template:smoke` again immediately | Pass | Run directory: `artifacts/template-smoke/runs/mq6j8leb-35dd52f0`. |

An earlier smoke iteration failed after all builds/tests because the new resilience semantic assertion looked for `https://example.invalid/` in the typed client file instead of the registration extension. The assertion was corrected before the two required clean back-to-back smoke runs.

## Docker Limitation

Live Testcontainers execution was not run in this environment because `docker` is not installed or not on `PATH`:

```text
docker: The term 'docker' is not recognized
```

This is not a default validation failure. Generated Docker-backed tests are intentionally opt-in and build as part of default smoke.

## OpenQuestions.md Updates

Two inferred decisions were added:

- `Q-20260609-002`: Keep generated Testcontainers tests opt-in because Docker availability is environment-dependent.
- `Q-20260609-003`: Keep fake authentication test-only until P1D-2 introduces real auth scaffolding.

No blockers were added.

## Remaining Gaps

Remaining work is outside P1D-1:

- P1D-2: JWT/auth scaffolding, permissions, and inbox pattern.
- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, and broader public polish.

Event sourcing, UI, project rename work, and architecture redesign were not introduced.

## Closure

P1D-1 is confirmed closed.

Recommended next step: start P1D-2 as a separate goal.
