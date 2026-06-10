# P1D-3B Post-Fix Verification

Date: 2026-06-10

## Summary

P1D-3B is confirmed closed after a verification-only run against the generated performance smoke test implementation.

No implementation fixes were made. This run did not start P1D-4 deployment skeleton work, UI work, event sourcing, message broker integration, public screenshots, badges, docs-site work, P2 public polish, or project renaming.

Fresh validation evidence:

- First smoke run: `artifacts/template-smoke/runs/mq7xk0n3-9fac7c99`
- Immediate second smoke run: `artifacts/template-smoke/runs/mq7xtygs-3fb1d6b4`
- Primary generated evidence source: `artifacts/template-smoke/runs/mq7xtygs-3fb1d6b4`

## P1D-3B Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Pro/advanced performance smoke project | Pass | `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `strict-enterprise`, and `taskhub` outputs include `tests/<App>.PerformanceSmokeTests`. |
| Core exclusion | Pass | `core-core` and `core-mediatr` outputs do not include `tests/<App>.PerformanceSmokeTests`; generated `docs/performance.md` explains the exclusion. |
| Generated solution wiring | Pass | Pro, advanced, strict enterprise, and TaskHub `.sln` files include the matching performance smoke project, so generated `dotnet test -c Release` runs it. |
| API test-host startup | Pass | `Api_test_host_startup_smoke_stays_within_loose_threshold` uses `WebApplicationFactory<Program>` and requests `/`. |
| Health/simple diagnostic endpoint | Pass | `Health_endpoint_response_smoke_stays_within_loose_threshold` measures warmed `/health` requests. |
| Authenticated protected path | Pass | `Authenticated_request_path_smoke_stays_within_loose_threshold` calls `/operations/outbox` through a test-local auth scheme and generated permission claims. |
| Generated CQRS request path | Pass | Starter outputs measure `/work-items/{id}`; TaskHub measures `/tasks/`. |
| OpenAPI generation | Pass | `OpenApi_document_generation_smoke_stays_within_loose_threshold` measures warmed `/openapi/v1.json` requests. |
| TaskHub sample path | Pass | `--sample taskhub` generated output uses the TaskHub tasks endpoint for the CQRS smoke path. |
| Timing implementation | Pass | Tests use `Stopwatch`, one warm-up iteration, three measured iterations, and best warmed sample diagnostics. |
| Loose named thresholds | Pass | `PerformanceSmokeThresholds` defines `HostStartup`, `SimpleRequest`, `AuthenticatedRequest`, `CqrsDispatchRequest`, `OpenApiGeneration`, `WarmupIterations`, and `MeasuredIterations`. |
| Diagnostic failure context | Pass | Failure messages include threshold values, all measured samples for warmed paths, and state that the checks are smoke diagnostics, not benchmarks or production certification. |
| No heavy benchmark infrastructure | Pass | Performance smoke assets contain no `BenchmarkDotNet` marker and use simple xUnit tests. |
| Runtime independence | Pass | Tests use EF InMemory and test-local fake auth; default validation does not require Docker, a broker, external identity provider, real JWT issuer, live database, or external services. |
| Fake auth isolation | Pass | Performance fake auth lives under `tests/<App>.PerformanceSmokeTests`; production `src` contains no `PerformanceSmokeTests` references. |
| Architecture boundaries | Pass | Generated architecture tests assert profile-specific project inclusion and that production projects do not reference performance smoke tests. |
| Smoke assertions | Pass | `tools/guardrails/check.mjs` contains `assertP1D3BPerformanceSmokeSemantics` and invokes it during template smoke. |
| Documentation and acceptance | Pass | `docs/performance.md`, `docs/testing.md`, `docs/acceptance-criteria.md`, and `specs/0001-aegis-template-core/acceptance.md` accurately describe P1D-3B. |
| Prior P1D behavior | Pass | Two full smoke runs passed generated core/pro/advanced, core and MediatR variants, contract tests, auth/permission tests, inbox tests, architecture tests, fake-auth/Testcontainers boundaries, and item-template coverage. |

## Generated Core/Pro/Advanced Evidence

Primary evidence came from `artifacts/template-smoke/runs/mq7xtygs-3fb1d6b4`.

Performance smoke project presence:

- `g/core-core/tests/Smoke.CoreCore.PerformanceSmokeTests`: absent.
- `g/core-mediatr/tests/Smoke.CoreMediatR.PerformanceSmokeTests`: absent.
- `g/pro-core/tests/Smoke.ProCore.PerformanceSmokeTests/Smoke.ProCore.PerformanceSmokeTests.csproj`: present.
- `g/pro-mediatr/tests/Smoke.ProMediatR.PerformanceSmokeTests/Smoke.ProMediatR.PerformanceSmokeTests.csproj`: present.
- `g/advanced-core/tests/Smoke.AdvancedCore.PerformanceSmokeTests/Smoke.AdvancedCore.PerformanceSmokeTests.csproj`: present.
- `g/advanced-mediatr/tests/Smoke.AdvancedMediatR.PerformanceSmokeTests/Smoke.AdvancedMediatR.PerformanceSmokeTests.csproj`: present.
- `g/strict-enterprise/tests/Smoke.StrictEnterprise.PerformanceSmokeTests/Smoke.StrictEnterprise.PerformanceSmokeTests.csproj`: present.
- `g/taskhub/tests/Aegis.TaskHub.PerformanceSmokeTests/Aegis.TaskHub.PerformanceSmokeTests.csproj`: present.

The root template uses the `profile == "core"` template modifier to exclude `tests/Aegis.Template.PerformanceSmokeTests/**`, so core remains lightweight.

## Performance Smoke Test Evidence

Representative template files:

- `templates/Aegis.Modulith.Templates/content/aegis-modulith/tests/Aegis.Template.PerformanceSmokeTests/PerformanceSmokeTests.cs`
- `templates/Aegis.Modulith.Templates/content/aegis-modulith/tests/Aegis.Template.PerformanceSmokeTests/Infrastructure/PerformanceSmokeAssertions.cs`
- `templates/Aegis.Modulith.Templates/content/aegis-modulith/tests/Aegis.Template.PerformanceSmokeTests/Infrastructure/PerformanceSmokeThresholds.cs`
- `templates/Aegis.Modulith.Templates/content/aegis-modulith/tests/Aegis.Template.PerformanceSmokeTests/Infrastructure/PerformanceSmokeWebApplicationFactory.cs`
- `templates/Aegis.Modulith.Templates/content/aegis-modulith/tests/Aegis.Template.PerformanceSmokeTests/Authentication/PerformanceSmokeAuthenticationHandler.cs`

Verified behavior:

- API startup is measured with `WebApplicationFactory<Program>` and a root request.
- `/health` is measured after warm-up.
- `/operations/outbox` is measured through test-local fake auth with `OperationsRead`.
- Starter CQRS path uses `WorkItemsRead` and `/work-items/{Guid}`.
- TaskHub CQRS path uses `TasksRead` and `/tasks/`.
- OpenAPI generation measures `/openapi/v1.json`.
- The test names explicitly use `smoke` and `loose_threshold`; they do not present themselves as benchmarks.

The generated project references only test and generated app dependencies needed for the smoke path: API, BuildingBlocks, Modules, `Microsoft.AspNetCore.Mvc.Testing`, EF InMemory, xUnit, and test SDK packages.

## Threshold And Stability Assessment

P1D-3B thresholds are loose enough for normal developer machines and CI:

- `HostStartup`: 15 seconds.
- `SimpleRequest`: 5 seconds.
- `AuthenticatedRequest`: 5 seconds.
- `CqrsDispatchRequest`: 5 seconds.
- `OpenApiGeneration`: 10 seconds.
- Warm-up iterations: 1.
- Measured iterations: 3.

The tests do not rely on exact millisecond expectations. For warmed request paths, the assertion evaluates the best sample from three measured samples and reports all samples on failure. This is a reasonable diagnostic smoke pattern for generated starter projects and does not look machine-sensitive.

The generated tests do not use BenchmarkDotNet, Docker, a broker, an external identity provider, a live database, a real JWT issuer, or external services. EF InMemory is configured inside a per-factory database name and internal InMemory provider, which keeps it isolated from the generated PostgreSQL provider and from other generated tests.

## Fake Auth Isolation Evidence

Performance fake auth is test-local:

- `tests/<App>.PerformanceSmokeTests/Authentication/PerformanceSmokeAuthenticationDefaults.cs`
- `tests/<App>.PerformanceSmokeTests/Authentication/PerformanceSmokeAuthenticationHandler.cs`
- Scheme: `Aegis.PerformanceSmoke`.
- Permissions header: `X-Performance-Smoke-Permissions`.

Production isolation checks:

- Generated production `src` search for `PerformanceSmokeTests` returned no matches for pro, advanced, or TaskHub outputs.
- Production `Program.cs` does not reference performance fake auth.
- Production project files do not reference performance smoke test projects.
- P1D-2A fake auth remains separately scoped to integration tests with the `Aegis.Test` scheme; performance smoke uses its own local scheme and does not alter that boundary.

## Smoke Assertions Reviewed

`tools/guardrails/check.mjs` includes `assertP1D3BPerformanceSmokeSemantics` and calls it during generated variant validation.

The smoke assertions cover:

- core exclusion of performance smoke assets;
- pro/advanced project presence;
- solution inclusion;
- API, BuildingBlocks, and Modules project references;
- `Microsoft.AspNetCore.Mvc.Testing` and EF InMemory usage;
- absence of `Testcontainers.PostgreSql`;
- no production project references to performance smoke tests;
- startup, health, authenticated request, CQRS request, and OpenAPI generation markers;
- named loose threshold markers;
- warmed sample diagnostics and `Stopwatch`;
- test-local fake auth;
- starter and TaskHub CQRS path coverage;
- denial markers for Docker, Testcontainers, broker technologies, external identity providers, external services, and `BenchmarkDotNet`;
- generated docs truthfulness;
- no unresolved template tokens in performance smoke assets.

The latest generated-output scan over pro and TaskHub performance smoke assets returned no unresolved `#if`, `#else`, `#endif`, `Aegis.Template`, `TODO_TEMPLATE`, or `__` markers.

## Documentation And Acceptance

Root docs are truthful:

- `docs/performance.md` says the tests are smoke diagnostics, not benchmarks, load tests, or production performance certification.
- It explains covered scenarios, loose thresholds, threshold names, safe adjustment guidance, and intentionally uncovered areas.
- It states no Docker, broker, external identity provider, external service, or real JWT issuer is required by default.
- `docs/testing.md` references performance smoke tests accurately and points to `docs/performance.md`.
- `docs/acceptance-criteria.md` marks P1D-3B complete with scoped criteria.
- `specs/0001-aegis-template-core/acceptance.md` marks P1D-3B complete with matching scoped criteria.
- `OpenQuestions.md` contains `Q-20260610-001`, which states performance smoke tests are diagnostic, loose-threshold checks for pro and advanced only.

Generated `docs/performance.md` also matches generated behavior:

- core output documents that the performance smoke project is not generated;
- pro/advanced output documents the generated project, covered paths, threshold names, run command, adjustment guidance, and out-of-scope runtime paths.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass. |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq7xk0n3-9fac7c99`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq7xtygs-3fb1d6b4`. |
| `npm run check` after writing this report | Pass. |

Observed generated test results included 5 passing performance smoke tests for pro and MediatR variants. Docker-backed Testcontainers tests remained skipped by default unless `AEGIS_RUN_TESTCONTAINERS=true` is set.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

Open blockers from `OpenQuestions.md`: none.

Relevant inferred decisions:

- `Q-20260610-001`: generate diagnostic performance smoke tests for pro and advanced only.
- `Q-20260609-010`: contract tests generated for pro/advanced only.
- `Q-20260609-004`: JWT bearer plus claim-policy scaffold defaults.
- `Q-20260609-005` through `Q-20260609-007`: inbox idempotency defaults and core exclusion.
- `Q-20260609-002`: generated Testcontainers tests remain opt-in.

## Remaining P1D-4/P2 Gaps

The following remain outside P1D-3B and were not started:

- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Validation Limitations

Default validation intentionally did not run live Docker-backed Testcontainers tests. No broker, external identity provider, real JWT issuer, live database, external service, load test, or deployed production environment was used.

This verification confirms generated scaffold behavior, default generated `dotnet test` behavior, smoke guardrail coverage, documentation, and architecture boundaries. It does not certify production performance.

## Closure

P1D-3B remains confirmed closed.
