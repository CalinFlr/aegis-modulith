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

## Pro And Advanced Integration Tests

Generated pro and advanced outputs include `tests/<App>.IntegrationTests`.

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

## HttpClient Resilience

Generated pro and advanced APIs register outbound `HttpClient` defaults through `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

The generated `SampleExternalStatusClient` is a placeholder pattern and uses `https://example.invalid/` so the template does not depend on a real external service.
