# Testing

## Default Validation

Default validation must not require Docker:

```bash
npm run check
npm run template:smoke
```

Generated solutions run:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

`template:smoke` builds and tests generated core, pro, advanced, and mediator variants. It also asserts P1D-1 semantics for generated pro and advanced outputs without starting Docker.
It now also asserts P1D-2A JWT/auth and permission-policy semantics without issuing real JWTs or calling an external identity provider.
It also asserts P1D-2B inbox semantics without requiring a broker or Docker.
It also asserts P1D-3A generated API and integration-contract test semantics without requiring Docker, a broker, an external identity provider, or external services.
It also asserts P1D-3B generated performance smoke test semantics for pro and advanced outputs without requiring Docker, a broker, an external identity provider, a real JWT issuer, or external services.
It also asserts P1D-4 deployment skeleton semantics for generated pro and advanced outputs without requiring Docker, Kubernetes, cloud credentials, registry credentials, a live database, or deployment secrets.

## Pro And Advanced Integration Tests

Generated pro and advanced outputs include `tests/<App>.IntegrationTests`.
They also include `tests/<App>.ContractTests` for API and integration-contract drift detection.
They also include `tests/<App>.PerformanceSmokeTests` for fast diagnostic runtime regression checks.

That test project contains:

- `PostgresContainerFixture`, which starts PostgreSQL through Testcontainers when Docker tests are enabled.
- `AegisWebApplicationFactory`, which overrides `ConnectionStrings:Postgres` for generated API tests.
- `DatabaseInitialization`, a buildable placeholder for future `Database.MigrateAsync` calls once generated modules add migrations.
- `ContainerizedPostgresSmokeTests`, an opt-in test proving the API host can start against containerized PostgreSQL.

The Docker-backed test is skipped by default. To run it on a machine with Docker:

```powershell
$env:AEGIS_RUN_TESTCONTAINERS = 'true'
dotnet test -c Release --filter "FullyQualifiedName~ContainerizedPostgresSmokeTests"
```

## Fake Authentication

Generated pro and advanced integration tests include fake authentication only in the test project.

The fake scheme is `Aegis.Test`. Test clients use these headers:

- `X-Test-User-Id`
- `X-Test-User-Name`
- `X-Test-Roles`
- `X-Test-Scopes`
- `X-Test-Permissions`

The handler maps user id and name to standard name claims, roles to `ClaimTypes.Role`, scopes to the `scope` claim, and permissions to the `permission` claim. Production `Program.cs` does not enable the fake scheme.

Generated permission-policy tests use fake auth to prove that a request with the required permission can access a protected endpoint and that a request without the required permission is rejected. These tests do not require real JWT issuance.

## Inbox Tests

Generated pro and advanced integration tests include fast inbox behavior tests under `tests/<App>.IntegrationTests/Inbox`.

They use EF InMemory and prove first acceptance, duplicate detection, processed-message idempotency, failure state, and single handler invocation for duplicate inputs. They do not require Docker or a message broker.

## Contract Tests

Generated pro and advanced contract tests live under `tests/<App>.ContractTests`.

They use semantic assertions over endpoint metadata, OpenAPI JSON, integration event metadata, and JSON round trips. They verify routes, methods, declared status codes, declared content types, JWT bearer OpenAPI metadata, named permission policies, fake-auth isolation, integration event type/version metadata, domain/integration event separation, and inbox payload identity fields.

These tests are not performance smoke tests. They do not require Docker, a broker, an external identity provider, real JWT issuance, or external services.

## Performance Smoke Tests

Generated pro and advanced performance smoke tests live under `tests/<App>.PerformanceSmokeTests`.

They use `Stopwatch`, warm-up requests, and intentionally loose thresholds to detect obvious regressions in API test-host startup, `/health`, authenticated protected requests through test-only fake auth, generated CQRS request paths, and OpenAPI document generation. They are smoke diagnostics, not benchmarks, load tests, or production performance certification.

The generated test factory replaces module persistence with EF InMemory and uses a test-local auth scheme, so the default run does not require Docker, a broker, an external identity provider, a real JWT issuer, a live database, or external services.

See [Performance](performance.md) for the threshold names and safe adjustment guidance.

## HttpClient Resilience

Generated pro and advanced APIs register outbound `HttpClient` defaults through `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

The generated `SampleExternalStatusClient` is a placeholder pattern and uses `https://example.invalid/` so the template does not depend on a real external service.
