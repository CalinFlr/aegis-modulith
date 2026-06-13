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

The handler maps user id and name to standard name claims, roles to `ClaimTypes.Role`, and scopes to the `scope` claim. Production `Program.cs` does not enable the fake scheme.

## HttpClient Resilience

Generated pro and advanced APIs register outbound `HttpClient` defaults through `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

The generated `SampleExternalStatusClient` is a placeholder pattern and uses `https://example.invalid/` so the template does not depend on a real external service.
