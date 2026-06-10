# Performance

#if (profile == "core")
## Core Profile

Core does not generate the pro/advanced performance smoke test project by default. It keeps the generated test surface lightweight and avoids pulling in `WebApplicationFactory`, fake-auth, EF InMemory, and diagnostic timing assets.

Choose `--profile pro` or `--profile advanced` when you want the generated performance smoke test foundation.
#else
## Generated Performance Smoke Tests

This profile includes `tests/Aegis.Template.PerformanceSmokeTests` in the solution.

These tests are not benchmarks, load tests, or production performance certification. They are fast diagnostic checks that help you notice obvious runtime regressions in generated starter projects.

The generated tests cover:

- API test host startup through `WebApplicationFactory<Program>`;
- `/health` response latency after warm-up;
- authenticated protected request latency through a test-only fake auth scheme;
- generated CQRS query request path latency;
- OpenAPI document generation latency.

#if (sample == "taskhub")
The CQRS smoke path uses the generated TaskHub tasks endpoint.
#else
The CQRS smoke path uses the generated starter WorkItems endpoint.
#endif

## Thresholds

Thresholds live in `PerformanceSmokeThresholds`:

- `HostStartup`
- `SimpleRequest`
- `AuthenticatedRequest`
- `CqrsDispatchRequest`
- `OpenApiGeneration`
- `WarmupIterations`
- `MeasuredIterations`

The thresholds are intentionally loose. The assertions use warmed samples and report all measured sample timings on failure. This keeps the tests deterministic enough for normal developer machines and CI while still catching clear regressions.

## Running

Run them with the normal generated test command:

```bash
dotnet test -c Release
```

You can also target only the performance smoke project:

```bash
dotnet test tests/Aegis.Template.PerformanceSmokeTests/Aegis.Template.PerformanceSmokeTests.csproj -c Release
```

## Adjusting Thresholds

Adjust thresholds only when the generated application intentionally adds startup or request-path work. Prefer raising the named threshold for the specific scenario instead of adding sleeps, broad retries, or machine-specific logic.

If a check becomes environment-sensitive in a real application, move the heavier path behind an opt-in script or test filter and keep the default smoke path fast.

## Not Covered

The generated performance smoke tests do not cover load testing, throughput, concurrency saturation, database performance, network calls, container-backed paths, broker processing, external identity provider latency, or production deployment behavior.

No Docker, broker, external identity provider, external service, or real JWT issuer is required by default. The generated tests use EF InMemory for request paths that touch module persistence and a test-local auth scheme for protected endpoints.
#endif
