# Performance

P1D-3B adds generated performance smoke tests for `pro` and `advanced` outputs.

These tests are not benchmarks, load tests, or production performance certification. They are fast diagnostic checks that help users notice obvious runtime regressions in generated starter projects.

## Generated Coverage

Generated `pro` and `advanced` solutions include `tests/<App>.PerformanceSmokeTests` in the solution, so normal generated validation runs them with:

```bash
dotnet test -c Release
```

The generated tests cover:

- API test host startup through `WebApplicationFactory<Program>`;
- `/health` response latency after warm-up;
- authenticated protected request latency through a test-only fake auth scheme;
- generated CQRS query request path latency;
- OpenAPI document generation latency.

When `--sample taskhub` is selected, the CQRS smoke path uses the generated TaskHub tasks endpoint. Otherwise it uses the starter WorkItems endpoint.

## Thresholds

Thresholds live in generated `PerformanceSmokeThresholds`:

- `HostStartup`
- `SimpleRequest`
- `AuthenticatedRequest`
- `CqrsDispatchRequest`
- `OpenApiGeneration`
- `WarmupIterations`
- `MeasuredIterations`

The thresholds are intentionally loose. The assertions use warmed samples and report all measured sample timings on failure. This keeps the tests deterministic enough for normal developer machines and CI while still catching clear regressions.

## Adjusting Thresholds

Adjust thresholds only when the generated application intentionally adds startup or request-path work. Prefer raising the named threshold for the specific scenario instead of adding sleeps, broad retries, or machine-specific logic.

If a check becomes environment-sensitive in a real application, move the heavier path behind an opt-in script or test filter and keep the default smoke path fast.

## Not Covered

The generated performance smoke tests do not cover load testing, throughput, concurrency saturation, database performance, network calls, container-backed paths, broker processing, external identity provider latency, or production deployment behavior.

No Docker, broker, external identity provider, external service, or real JWT issuer is required by default. The generated tests use EF InMemory for request paths that touch module persistence and a test-local auth scheme for protected endpoints.
