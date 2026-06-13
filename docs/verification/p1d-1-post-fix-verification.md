# P1D-1 Post-Fix Verification

Date: 2026-06-09

## Summary

P1D-1 is **not confirmed closed** in this verification run.

The generated pro and advanced code foundations for Testcontainers PostgreSQL tests, fake authentication, and Microsoft-first HttpClient resilience are present, build, and pass default smoke without requiring Docker. However, generated documentation still contains unprocessed template condition directives, and generated `core` documentation describes pro-only integration-test assets that do not exist in the core output. The smoke runner also does not currently fail on those unresolved template directives in generated main-template docs.

No implementation fixes were made. P1D-2 JWT/auth/permissions/inbox work, P1D-3 contract/performance tests, P1D-4 deployment skeleton, UI, event sourcing, public polish, and project rename work were not started.

Reviewed implementation commits:

- `df971ed`: `test: add pro advanced testcontainers foundation`
- `9a383ff`: `test: add fake auth integration test foundation`
- `bc3c5b7`: `feat: add pro advanced httpclient resilience defaults`
- `39d8346`: `test: assert p1d feature depth in smoke`
- `aeec56f`: `docs: verify p1d feature depth foundation`

## P1D-1 Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Testcontainers foundation | Pass | Pro/advanced outputs include `tests/<App>.IntegrationTests`, `Testcontainers.PostgreSql`, `Microsoft.AspNetCore.Mvc.Testing`, `PostgresContainerFixture`, `AegisWebApplicationFactory`, `DockerFactAttribute`, `DatabaseInitialization`, and `ContainerizedPostgresSmokeTests`. |
| Default smoke does not require Docker | Pass | Generated Docker-backed tests are skipped unless `AEGIS_RUN_TESTCONTAINERS=true`; default `dotnet test` passed with one integration test skipped in pro/advanced outputs. |
| Connection override and initialization path | Pass | `AegisWebApplicationFactory` overrides only `ConnectionStrings:Postgres` in the test host; `DatabaseInitialization` is buildable and documents the future `Database.MigrateAsync` handoff. |
| Fake auth test foundation | Pass | Fake auth is under the generated integration test project, uses scheme `Aegis.Test`, supports `X-Test-*` headers, provides test users and authenticated-client helpers, and has a smoke test mapping headers to claims. |
| Fake auth production isolation | Pass | Generated production `src/<App>.Api/Program.cs` had no `FakeAuthentication`, `Aegis.Test`, or `X-Test-*` markers. |
| HttpClient resilience defaults | Pass | Pro/advanced API projects reference `Microsoft.Extensions.Http.Resilience`; `AddAegisOutboundHttpClients` wires `ConfigureHttpClientDefaults`, `AddStandardResilienceHandler`, and `SampleExternalStatusClient` with `https://example.invalid/`. |
| Core profile exclusion | Pass | Generated `core-core` has no integration test project, no pro `Http` folder, no Testcontainers package reference, and no resilience API package reference. |
| Template option behavior | Pass | Smoke covered core/pro/advanced and mediator core/MediatR variants; generated architecture tests still passed. |
| P1A/P1B/P1C regression coverage | Pass | Smoke still runs generated guardrails, item-template compatibility, and 23 architecture tests per generated solution. |
| Documentation truthfulness | Fail | Generated `core-core/docs/testing.md` retains `#if/#else/#endif` markers and says `tests/Smoke.CoreCore.IntegrationTests` exists even though core output correctly omits that project. Pro/advanced generated `docs/testing.md` also retain the conditional markers and core-only section. |
| Smoke unresolved-token coverage | Fail | `npm run template:smoke` passed while latest generated outputs still contained literal template condition markers such as `#if`, `#else`, and `#endif` in generated docs and AGENTS/README files. P1D-1 smoke assertions cover generated P1D-1 code shape but not unresolved main-template text directives. |

## Generated Output Evidence

Latest smoke run used for generated-output inspection:

```text
artifacts/template-smoke/runs/mq6kcfc4-24096fe8
```

The first immediate smoke run also passed:

```text
artifacts/template-smoke/runs/mq6k3w8x-2180ede4
```

Representative latest generated outputs:

```text
g/core-core
g/pro-core
g/advanced-core
g/pro-mediatr
g/advanced-mediatr
```

Observed P1D-1 file-shape evidence:

- `core-core`: no `tests/<App>.IntegrationTests`, no `src/<App>.Api/Pro/Http`, no resilience package reference.
- `pro-core`: includes `tests/Smoke.ProCore.IntegrationTests` and `src/Smoke.ProCore.Api/Pro/Http`.
- `advanced-core`: includes `tests/Smoke.AdvancedCore.IntegrationTests` and `src/Smoke.AdvancedCore.Api/Pro/Http`.
- `pro-mediatr` and `advanced-mediatr`: build and test with the same integration-test foundations.

Package and code evidence from the latest run:

- `pro-core/Directory.Packages.props` and `advanced-core/Directory.Packages.props` include `Testcontainers.PostgreSql`, `Microsoft.AspNetCore.Mvc.Testing`, and `Microsoft.Extensions.Http.Resilience`.
- `pro-core/tests/Smoke.ProCore.IntegrationTests/Smoke.ProCore.IntegrationTests.csproj` references `Testcontainers.PostgreSql`.
- `advanced-core/tests/Smoke.AdvancedCore.IntegrationTests/Smoke.AdvancedCore.IntegrationTests.csproj` references `Testcontainers.PostgreSql`.
- `PostgresContainerFixture.cs` uses `PostgreSqlBuilder` and disposes the container.
- `DockerFactAttribute.cs` gates Docker-backed tests on `AEGIS_RUN_TESTCONTAINERS=true`.
- `AegisWebApplicationFactory.cs` overrides `ConnectionStrings:Postgres` only inside the test host.
- `FakeAuthenticationDefaults.cs`, `FakeAuthenticationHandler.cs`, `FakeAuthenticationHeaders.cs`, `TestUsers.cs`, and `AuthenticatedClientExtensions.cs` exist only under generated integration tests.
- `ResilientHttpClientServiceCollectionExtensions.cs` uses `AddStandardResilienceHandler`.

Default test evidence from both smoke runs:

```text
Passed: 23, Skipped: 0, Total: 23 - architecture tests
Passed: 1, Skipped: 1, Total: 2 - pro/advanced integration tests
```

The skipped integration test is `ContainerizedPostgresSmokeTests.Api_host_starts_against_containerized_postgres`, which is intentionally Docker-gated.

## Smoke Assertions Reviewed

Reviewed `tools/guardrails/check.mjs`:

- `assertP1DFeatureDepthSemantics` checks pro/advanced Testcontainers package references, WebApplicationFactory package references, integration test project inclusion, PostgreSQL fixture, Docker opt-in fact, connection-string override, database initialization placeholder, fake auth files, fake auth production isolation, resilience package/reference/registration, sample typed client, and core exclusions.
- The smoke matrix includes `core-core`, `core-mediatr`, `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `taskhub`, strict enterprise, AI variants, docs/skills variants, and guardrails-off variants.
- Smoke uses fresh run directories under `artifacts/template-smoke/runs/<run-id>` and isolated `DOTNET_CLI_HOME`/`NUGET_PACKAGES`, so two immediate runs validate idempotent generation.

Coverage gap:

- `assertNoTemplateTokens` is used for item-template outputs, but the main generated outputs are not scanned broadly for unresolved text directives.
- The latest smoke run passed even though `rg "#if|#else|#endif" artifacts/template-smoke/runs/mq6kcfc4-24096fe8/g` found generated markdown and AGENTS/README files with literal template directives.

## Documentation and Acceptance

Repository docs:

- `docs/testing.md` accurately explains the default no-Docker validation path, Docker opt-in Testcontainers path, fake auth headers/claims, and HttpClient resilience behavior.
- `OpenQuestions.md` contains `Q-20260609-002` for opt-in Testcontainers tests and `Q-20260609-003` for test-only fake auth.
- `docs/acceptance-criteria.md` and `specs/0001-aegis-template-core/acceptance.md` mark P1D-1 complete and do not claim live Docker validation was run.

Generated docs:

- Generated `docs/testing.md` does not currently match generated behavior for core because it describes `tests/Smoke.CoreCore.IntegrationTests` even though that project is correctly excluded.
- Generated `docs/testing.md` in pro and advanced also retains `#if/#else/#endif` markers, so it is not production-quality generated documentation.

Because documentation truthfulness and unresolved generated directives are explicit verification criteria for this run, P1D-1 should not remain marked confirmed closed until those are corrected and smoke covers them.

## Checks Run

| Command | Result | Evidence |
| --- | --- | --- |
| `npm run check` | Pass | `ai instructions`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, and `security` passed. Rerun after adding this report also passed. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq6k3w8x-2180ede4`. |
| `npm run template:smoke` again immediately | Pass | Run directory: `artifacts/template-smoke/runs/mq6kcfc4-24096fe8`. |
| Docker availability check | Not available | `Get-Command docker` returned no executable. Live Testcontainers execution was not run. |

Default validation did not require Docker.

## Remaining P1D-2/P1D-3/P1D-4/P2 Gaps

Remaining work outside P1D-1:

- P1D-2: real JWT/auth scaffolding, permissions hardening, and inbox pattern.
- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

No broker, workflow engine, distributed messaging scope, event sourcing, UI, or project rename work was introduced in this run.

## Validation Limitations

- Docker is not installed or not on `PATH`, so the live Testcontainers path was not executed.
- The smoke runner validates P1D-1 code shape and build/test behavior but does not currently detect unresolved text-template directives in generated main-template docs.
- This run intentionally did not implement fixes.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

No new true blocker or inferred decision was identified. The failure is a concrete verification finding, not an unresolved human decision.

Existing relevant inferred decisions:

- `Q-20260609-002`: generated Testcontainers tests stay opt-in.
- `Q-20260609-003`: fake authentication stays test-only until P1D-2.

`OpenQuestions.md` still lists no blockers.

## Closure

P1D-1 is **not confirmed closed**.

Recommended next step: fix generated markdown/text conditional processing and extend smoke to fail on unresolved main-template directives, then rerun this P1D-1 verification before starting P1D-2.
