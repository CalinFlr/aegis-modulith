# Authentication And Permissions

#if (profile != "core")
This profile includes JWT bearer authentication and permission-policy scaffolding.

This is not a bundled identity provider, user database, role manager, login UI, or token issuer. Configure your own issuer, audience, and signing key before relying on protected endpoints.

## JWT Configuration

The generated API reads JWT settings from `Authentication:Jwt`.

Example configuration:

```json
{
  "Authentication": {
    "Jwt": {
      "Issuer": "https://issuer.example",
      "Audience": "Aegis.Template",
      "SigningKey": "replace-with-a-user-supplied-development-or-production-key",
      "RequireHttpsMetadata": true
    }
  }
}
```

Do not commit real signing keys. Use user secrets, environment variables, a secret manager, or the deployment platform's secret store for real values.

If issuer, audience, or signing key is missing, JWT validation uses a reject-all configuration. The app can start, but protected endpoints do not accept arbitrary bearer tokens.

## Permissions

Permissions are represented as claims. Generated policies check the `permission` claim and also accept a matching `scope` claim for compatibility with identity providers that emit scopes.

Generated examples include:

- `work-items:read`
- `work-items:write`
- `tasks:read`
- `tasks:write`
- `operations:read`
#if (profile == "advanced")
- `advanced:read`
#endif

Policy names are centralized in `AegisAuthorizationPolicies`; permission values are centralized in `AegisPermissions`.

To add a permission:

1. Add a permission value to `AegisPermissions`.
2. Add a policy name to `AegisAuthorizationPolicies`.
3. Register the policy in `AddAegisPermissionPolicies`.
4. Require it on an endpoint with `RequireAuthorization(AegisAuthorizationPolicies.YourPolicy)`.

## Fake Auth In Tests

The integration test project uses fake authentication instead of issuing real JWTs.

The fake auth handler is generated only under `tests/Aegis.Template.IntegrationTests`. Production `Program.cs` does not wire the fake scheme.

Test clients can provide roles, scopes, and permissions through `X-Test-*` headers, including `X-Test-Permissions`, so permission policies can be tested without an external identity provider.

## Contract Checks

Generated contract tests assert that permission policy constants are registered as named policies and that protected endpoints expose those named policies in endpoint metadata.

They also assert that OpenAPI exposes JWT bearer security metadata and that fake-auth headers and the `Aegis.Test` scheme are not part of the production API contract.
#else
The core profile does not include JWT bearer authentication, auth middleware, policy registration, fake auth, or generated integration tests.

Core does include minimal shared permission constants in BuildingBlocks so modules and item templates can use stable names if the project later opts into pro or advanced auth scaffolding.
#endif
