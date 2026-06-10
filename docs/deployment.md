# Deployment

P1D-4 adds a practical deployment skeleton for generated `pro` and `advanced` API outputs.

This is scaffolding, not full production infrastructure. It does not create cloud resources, push container images, configure Kubernetes, provision PostgreSQL, issue JWTs, install an identity provider, add a broker, or require an observability backend.

## Generated Assets

Generated `pro` and `advanced` outputs include:

- `Dockerfile` for the generated API project;
- `.dockerignore`;
- `docker-compose.yml` for local API plus PostgreSQL development only;
- `.env.example` with environment-variable placeholders;
- `src/<App>.Api/appsettings.Production.json` with empty production placeholders;
- `docs/deployment.md`;
- a GitHub Actions CI container-build job and a disabled deployment placeholder.

The `core` profile remains lightweight and excludes these container/deployment assets by default.

## Container Image

Build the API container from a generated `pro` or `advanced` output:

```bash
docker build -t my-aegis-api:local .
```

Run it with environment variables supplied by your shell, platform, or secret store. Do not commit real values.

```bash
docker run --rm -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Postgres="Host=postgres;Port=5432;Database=app;Username=app;Password=replace-me" \
  -e Authentication__Jwt__Issuer="https://issuer.example" \
  -e Authentication__Jwt__Audience="aegis-api" \
  -e Authentication__Jwt__SigningKey="replace-with-user-supplied-secret" \
  my-aegis-api:local
```

The container exposes port `8080` and uses `/health` for the Docker healthcheck.

## Local Compose

`docker-compose.yml` is local/dev oriented. It starts only the API and PostgreSQL. It does not represent a production deployment and does not include a broker, identity provider, Kubernetes, ingress, TLS, or monitoring stack.

Use `.env.example` as a shape reference, then provide real local values through an untracked `.env` file or shell variables.

## Configuration

Production values should come from environment variables or the deployment platform's secret store.

Required user-provided values include:

- `ConnectionStrings__Postgres`;
- `Authentication__Jwt__Issuer`;
- `Authentication__Jwt__Audience`;
- `Authentication__Jwt__SigningKey`;
- `AllowedHosts`.

Optional deployment values include:

- `Logging__LogLevel__Default`;
- `OTEL_SERVICE_NAME`;
- `OTEL_EXPORTER_OTLP_ENDPOINT`;
- `Inbox__EnableBackgroundProcessor`;
- resilience settings such as `Resilience__DefaultTimeoutSeconds`.

The generated `appsettings.Production.json` intentionally uses empty placeholders for secrets. Missing JWT issuer, audience, or signing key keeps protected endpoints from accepting arbitrary bearer tokens.

## CI And Deployment Placeholder

Generated pro and advanced CI separates validation from deployment:

- the `dotnet` job restores, builds, tests, and runs generated guardrails when enabled;
- the `container` job builds the Docker image without registry credentials;
- the `deployment-placeholder` job is manual and gated by `vars.ENABLE_DEPLOYMENT_PLACEHOLDER == 'true'`.

No registry, organization, repository, cloud provider, or deployment target is hardcoded. Add registry login, image push, and provider-specific deploy steps in the generated repository when you choose a platform.

## Health And Observability

The generated API maps `/health`. By default this is a basic process health endpoint. It does not require PostgreSQL or an external service, so local tests and smoke validation stay stable.

OpenTelemetry ASP.NET Core instrumentation is already wired. Configure exporters later with environment variables or platform-specific hosting code. No collector is required by default.
