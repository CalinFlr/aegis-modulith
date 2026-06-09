# Authentication And Permissions

P1D-2A adds production-minded authentication and authorization scaffolding for generated `pro` and `advanced` profiles.

This is JWT bearer and permission-policy scaffolding, not a bundled identity provider, user database, role manager, login UI, or token issuer.

## Profile Behavior

- `core` stays lightweight and does not include JWT bearer package references, auth middleware wiring, policy registration, or fake auth.
- `pro` includes JWT bearer authentication, permission policy registration, and sample protected endpoints.
- `advanced` includes the same pro auth foundation and adds an advanced protected endpoint.
- Minimal permission-name constants live in the generated BuildingBlocks project so module endpoints can reference named policies without depending on the API project.

## JWT Configuration

Generated pro and advanced APIs read JWT settings from:

```json
{
  "Authentication": {
    "Jwt": {
      "Issuer": "https://issuer.example",
      "Audience": "aegis-api",
      "SigningKey": "replace-with-a-user-supplied-development-or-production-key",
      "RequireHttpsMetadata": true
    }
  }
}
```

Do not commit real signing keys. Use user secrets, environment variables, a secret manager, or the deployment platform's secret store for real values.

If issuer, audience, or signing key is missing, generated JWT validation uses a reject-all configuration. The app can start, but protected endpoints do not accept arbitrary bearer tokens.

## Permissions

Permissions are represented as claims. The generated policy scaffold checks the `permission` claim and also accepts a matching `scope` claim for compatibility with identity providers that emit scopes.

Generated examples include:

- `work-items:read`
- `work-items:write`
- `tasks:read`
- `tasks:write`
- `operations:read`
- `advanced:read`

Policy names are centralized in `AegisAuthorizationPolicies`; permission values are centralized in `AegisPermissions`.

To add a permission:

1. Add a permission value to `AegisPermissions`.
2. Add a policy name to `AegisAuthorizationPolicies`.
3. Register the policy in `AddAegisPermissionPolicies`.
4. Require it on an endpoint with `RequireAuthorization(AegisAuthorizationPolicies.YourPolicy)`.

## Fake Auth In Tests

Generated pro and advanced integration tests use fake authentication instead of issuing real JWTs.

The fake auth handler is generated only under `tests/<App>.IntegrationTests`. Production `Program.cs` does not wire the fake scheme.

Test clients can provide roles, scopes, and permissions through `X-Test-*` headers, including `X-Test-Permissions`, so permission policies can be tested without an external identity provider.
