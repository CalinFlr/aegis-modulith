# Testing

## Default Validation

Run the generated solution checks without requiring Docker:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

Default template smoke asserts P1D-4 deployment skeleton semantics without requiring Docker, Kubernetes, cloud credentials, registry credentials, a live database, or deployment secrets.

#if (guardrails != "off")
Generated guardrails can also run:

```bash
npm run check
```

#endif
#if (profile != "core")
## Pro And Advanced Integration Tests

This profile includes `tests/Aegis.Template.IntegrationTests`, `tests/Aegis.Template.ContractTests`, and `tests/Aegis.Template.PerformanceSmokeTests`.

That test project contains:

- `PostgresContainerFixture`, which starts PostgreSQL through Testcontainers when Docker tests are enabled.
- `AegisWebApplicationFactory`, which overrides `ConnectionStrings:Postgres` for generated API tests.
- `DatabaseInitialization`, a buildable placeholder for future `Database.MigrateAsync` calls once modules add migrations.
- `ContainerizedPostgresSmokeTests`, an opt-in test proving the API host can start against containerized PostgreSQL.

The Docker-backed test is skipped by default. To run it on a machine with Docker:

```powershell
$env:AEGIS_RUN_TESTCONTAINERS = 'true'
dotnet test -c Release --filter "FullyQualifiedName~ContainerizedPostgresSmokeTests"
```

## Fake Authentication

Fake authentication is available only in the integration test project.

The fake scheme is `Aegis.Test`. Test clients use these headers:

- `X-Test-User-Id`
- `X-Test-User-Name`
- `X-Test-Roles`
- `X-Test-Scopes`
- `X-Test-Permissions`

The handler maps user id and name to standard name claims, roles to `ClaimTypes.Role`, scopes to the `scope` claim, and permissions to the `permission` claim. Production `Program.cs` does not enable the fake scheme.

Generated permission-policy tests use fake auth to prove that a request with the required permission can access a protected endpoint and that a request without the required permission is rejected. These tests do not require real JWT issuance.

## Inbox Tests

Generated inbox behavior tests live under `tests/Aegis.Template.IntegrationTests/Inbox`.

They use EF InMemory and prove first acceptance, duplicate detection, processed-message idempotency, failure state, and single handler invocation for duplicate inputs. They do not require Docker or a message broker.

## Contract Tests

Generated contract tests live under `tests/Aegis.Template.ContractTests`.

They use semantic assertions over endpoint metadata, OpenAPI JSON, integration event metadata, and JSON round trips. They verify routes, methods, declared status codes, declared content types, JWT bearer OpenAPI metadata, named permission policies, fake-auth isolation, integration event type/version metadata, domain/integration event separation, and inbox payload identity fields.

These tests are not performance smoke tests. They do not require Docker, a broker, an external identity provider, real JWT issuance, or external services.

## Performance Smoke Tests

Generated performance smoke tests live under `tests/Aegis.Template.PerformanceSmokeTests`.

They use `Stopwatch`, warm-up requests, and intentionally loose thresholds to detect obvious regressions in API test-host startup, `/health`, authenticated protected requests through test-only fake auth, generated CQRS request paths, and OpenAPI document generation. They are smoke diagnostics, not benchmarks, load tests, or production performance certification.

The generated test factory replaces module persistence with EF InMemory and uses a test-local auth scheme, so the default run does not require Docker, a broker, an external identity provider, a real JWT issuer, a live database, or external services.

See [Performance](performance.md) for the threshold names and safe adjustment guidance.

## HttpClient Resilience

The generated API registers outbound `HttpClient` defaults through `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

`SampleExternalStatusClient` is a placeholder pattern and uses `https://example.invalid/` so the generated app does not depend on a real external service.
#else
## Core Profile

The core profile keeps the test surface lightweight. It includes architecture tests, but it does not include Testcontainers, fake authentication infrastructure, performance smoke test assets, or pro HttpClient resilience files.
#endif
