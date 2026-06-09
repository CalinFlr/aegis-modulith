# Testing

## Default Validation

Run the generated solution checks without requiring Docker:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

#if (guardrails != "off")
Generated guardrails can also run:

```bash
npm run check
```

#endif
#if (profile != "core")
## Pro And Advanced Integration Tests

This profile includes `tests/Aegis.Template.IntegrationTests`.

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

## HttpClient Resilience

The generated API registers outbound `HttpClient` defaults through `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

`SampleExternalStatusClient` is a placeholder pattern and uses `https://example.invalid/` so the generated app does not depend on a real external service.
#else
## Core Profile

The core profile keeps the test surface lightweight. It includes architecture tests, but it does not include Testcontainers, fake authentication infrastructure, or pro HttpClient resilience files.
#endif
