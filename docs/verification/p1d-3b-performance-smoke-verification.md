# P1D-3B Performance Smoke Verification

Date: 2026-06-10

## Summary

P1D-3B adds generated performance smoke tests for `pro` and `advanced` outputs.

This work did not start P1D-4 deployment skeleton work, UI work, event sourcing, broker integration, public screenshots, badges, docs-site work, P2 public polish, or project renaming.

Fresh validation evidence:

- First passing smoke run: `artifacts/template-smoke/runs/mq7qvfll-a31de3b4`.
- Immediate second passing smoke run: `artifacts/template-smoke/runs/mq7rczu7-f1320619`.
- Primary generated evidence source: `artifacts/template-smoke/runs/mq7rczu7-f1320619`.

## P1D-3B Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced performance smoke project | Pass | `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `strict-enterprise`, and `taskhub` outputs include `tests/<App>.PerformanceSmokeTests`. |
| Core exclusion | Pass | `core-core` and `core-mediatr` outputs do not include `tests/<App>.PerformanceSmokeTests`; generated `docs/performance.md` explains core exclusion. |
| Generated solution wiring | Pass | Pro, advanced, strict enterprise, and TaskHub `.sln` files include the matching `PerformanceSmokeTests` project, so default `dotnet test` runs it. |
| Startup smoke | Pass | `Api_test_host_startup_smoke_stays_within_loose_threshold` starts `WebApplicationFactory<Program>()` and requests `/`. |
| Health/simple request smoke | Pass | `Health_endpoint_response_smoke_stays_within_loose_threshold` measures warmed `/health` requests. |
| Authenticated path smoke | Pass | `Authenticated_request_path_smoke_stays_within_loose_threshold` calls `/operations/outbox` through a test-local auth scheme and permission claims. |
| CQRS request path smoke | Pass | Starter outputs measure `/work-items/{id}` with EF InMemory; TaskHub measures `/tasks/`. Core and MediatR mediator variants build/test where applicable. |
| OpenAPI generation smoke | Pass | `OpenApi_document_generation_smoke_stays_within_loose_threshold` measures warmed `/openapi/v1.json` requests. |
| Loose diagnostic thresholds | Pass | Generated `PerformanceSmokeThresholds` defines `HostStartup`, `SimpleRequest`, `AuthenticatedRequest`, `CqrsDispatchRequest`, `OpenApiGeneration`, `WarmupIterations`, and `MeasuredIterations`, with comments stating thresholds are intentionally loose smoke thresholds. |
| No Docker or live database by default | Pass | Performance smoke tests use EF InMemory and no `Testcontainers.PostgreSql` reference. |
| No broker/external auth/external service dependency | Pass | Smoke assertions deny broker, external identity provider, and `BenchmarkDotNet` markers in performance smoke assets. The generated tests use test-local auth and no external service. |
| Production boundary | Pass | Generated architecture tests assert production projects do not reference performance smoke tests. |
| No unresolved template tokens | Pass | `assertNoTemplateDirectives` ran across generated outputs; P1D-3B-specific smoke also checks perf test files for unresolved template tokens. |
| P1D-3A/P1D-2/P1D-1 behavior | Pass | Full smoke still ran generated architecture, integration, contract, inbox, auth, fake-auth, and Docker-gated Testcontainers tests. |

## Generated Output Evidence

Representative generated projects from `mq7rczu7-f1320619`:

- `g/pro-core/tests/Smoke.ProCore.PerformanceSmokeTests/Smoke.ProCore.PerformanceSmokeTests.csproj`
- `g/pro-mediatr/tests/Smoke.ProMediatR.PerformanceSmokeTests/Smoke.ProMediatR.PerformanceSmokeTests.csproj`
- `g/advanced-core/tests/Smoke.AdvancedCore.PerformanceSmokeTests/Smoke.AdvancedCore.PerformanceSmokeTests.csproj`
- `g/advanced-mediatr/tests/Smoke.AdvancedMediatR.PerformanceSmokeTests/Smoke.AdvancedMediatR.PerformanceSmokeTests.csproj`
- `g/strict-enterprise/tests/Smoke.StrictEnterprise.PerformanceSmokeTests/Smoke.StrictEnterprise.PerformanceSmokeTests.csproj`
- `g/taskhub/tests/Aegis.TaskHub.PerformanceSmokeTests/Aegis.TaskHub.PerformanceSmokeTests.csproj`

Core exclusion evidence:

- `g/core-core/tests/Smoke.CoreCore.PerformanceSmokeTests` is absent.
- `g/core-mediatr/tests/Smoke.CoreMediatR.PerformanceSmokeTests` is absent.
- Core generated `docs/performance.md` says core does not generate the pro/advanced performance smoke test project by default.

## Test Design

The generated tests use `Stopwatch` and warmed samples. Failures report the threshold and all measured samples, and the failure message states the checks are smoke diagnostics, not benchmarks or production performance certification.

The generated test factory:

- uses `WebApplicationFactory<Program>`;
- replaces the generated module DbContext with EF InMemory;
- isolates the EF InMemory provider from the app's Npgsql provider;
- uses a test-local `Aegis.PerformanceSmoke` auth scheme;
- supplies permission and scope claims through `X-Performance-Smoke-Permissions`;
- does not require Docker, a broker, a live database, an external identity provider, a real JWT issuer, or external services.

## Smoke Assertions

Root `tools/guardrails/check.mjs` now includes `assertP1D3BPerformanceSmokeSemantics` and invokes it for generated variants.

The assertions cover:

- core exclusion and generated docs truthfulness;
- pro/advanced project presence and solution inclusion;
- references to API, BuildingBlocks, and Modules projects;
- `Microsoft.AspNetCore.Mvc.Testing` and EF InMemory usage;
- no `Testcontainers.PostgreSql`;
- no production project references to performance smoke tests;
- startup, health, authenticated request, CQRS request, and OpenAPI generation test markers;
- loose named threshold markers;
- warmed-sample diagnostics;
- no broker, external identity provider, or `BenchmarkDotNet` markers;
- no unresolved template tokens in generated performance smoke assets.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass after increasing the tool timeout; default 120s tool timeout was too short for this workspace. |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq7qvfll-a31de3b4`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq7rczu7-f1320619`. |
| `npm run check` after writing this report | Pass. |

Smoke notes:

- `MSBUILDDISABLENODEREUSE=1` was set for the passing smoke invocations to avoid orphaned MSBuild node-reuse processes after earlier timed-out smoke attempts. This does not change generated template behavior.
- Docker-backed Testcontainers tests remained skipped by default.
- Generated performance smoke tests ran during default generated `dotnet test`.

## OpenQuestions.md Updates

`OpenQuestions.md` was updated with:

- `Q-20260610-001`: generate diagnostic performance smoke tests for pro and advanced only, with loose thresholds and default runtime independence.

Open blockers from `OpenQuestions.md`: none.

Relevant inferred decisions:

- `Q-20260610-001`: P1D-3B diagnostic performance smoke tests for pro/advanced only.
- `Q-20260609-010`: contract tests generated for pro/advanced only.
- `Q-20260609-004`: JWT bearer plus claim-policy scaffold defaults.
- `Q-20260609-005` through `Q-20260609-007`: inbox idempotency defaults and core exclusion.
- `Q-20260609-002`: generated Testcontainers tests remain opt-in.

## Spec Folder Used

Updated:

- `specs/0001-aegis-template-core/acceptance.md`

## Remaining P1D-4/P2 Gaps

The following remain outside P1D-3B and were not started:

- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Validation Limitations

Default validation did not run live Docker-backed Testcontainers tests because they are intentionally skipped unless `AEGIS_RUN_TESTCONTAINERS=true` is set. No broker, external identity provider, real JWT issuer, live database, or external service was used.

Early smoke attempts failed while implementing P1D-3B because of generated compile errors, permission-claim misuse, EF provider isolation, and one smoke assertion location mismatch. Those were fixed before the two passing smoke runs listed above.

## Closure

P1D-3B is confirmed closed.
