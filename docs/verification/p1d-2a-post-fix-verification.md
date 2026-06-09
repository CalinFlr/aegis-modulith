# P1D-2A Post-Fix Verification

Date: 2026-06-09

## Summary

P1D-2A is confirmed closed after a verification-only run.

No fixes were implemented. P1D-2B inbox pattern, P1D-3 contract/performance tests, P1D-4 deployment skeleton, UI, event sourcing, public screenshots, badges, docs site work, P2 public polish, and project renaming were not started.

Fresh validation evidence:

- First smoke run: `artifacts/template-smoke/runs/mq6qmcxj-3e014ecf`
- Immediate second smoke run: `artifacts/template-smoke/runs/mq6qu74l-43fe5481`
- Primary generated evidence source: `artifacts/template-smoke/runs/mq6qu74l-43fe5481`

## P1D-2A Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| JWT bearer package references in pro/advanced | Pass | `pro-core` and `advanced-core` generated `Directory.Packages.props` include `Microsoft.AspNetCore.Authentication.JwtBearer`; both generated API `.csproj` files reference the package. |
| Auth options and configuration scaffold | Pass | `src/<App>.Api/Pro/Auth/AegisJwtOptions.cs` defines `Authentication:Jwt`; `AegisJwtAuthenticationServiceCollectionExtensions.cs` configures typed options and `AddJwtBearer`. |
| No real auth secrets in generated sample config | Pass | Generated `appsettings.json` does not include JWT issuer, audience, or signing key values. Generated auth docs show only `https://issuer.example` and `replace-with-a-user-supplied-development-or-production-key`. |
| Missing JWT config is safe | Pass | `CreateRejectAllValidationParameters()` validates issuer, audience, and signing key and uses `RandomNumberGenerator.GetBytes(64)`, so missing config does not accept arbitrary bearer tokens. |
| Program middleware order | Pass | Pro `Program.cs` lines 29-30 and advanced `Program.cs` lines 31-32 call `UseAuthentication()` before `UseAuthorization()`. |
| Docs explain JWT setup accurately | Pass | Root and generated `docs/authentication.md` describe issuer, audience, signing key, secret storage, and reject-all missing-config behavior. |
| No identity provider dependency | Pass | Template source search found no `AddIdentity`, IdentityServer, OpenIddict, Duende, Keycloak, user manager, role manager, or login UI scaffolding. Only JWT bearer validation and `Microsoft.IdentityModel.Tokens` are present. |
| Permission constants and named policies | Pass | Generated BuildingBlocks includes `AegisPermissions`, `AegisAuthorizationPolicies`, and `AegisPermissionClaimTypes`; pro auth registers named policies in `AddAegisPermissionPolicies`. |
| Claim-based transparent policies | Pass | `AegisPermissionPolicyServiceCollectionExtensions.cs` checks `permission` claims and compatible `scope` claims with `RequireAssertion`. |
| Generated endpoint policy usage | Pass | Pro operations endpoint requires `OperationsRead`; starter `WorkItems` endpoints require `WorkItemsWrite` and `WorkItemsRead`; advanced endpoint requires `AdvancedRead`; TaskHub smoke coverage checks task read/write policies. |
| Named constants instead of magic strings | Pass | Generated endpoint code uses `AegisAuthorizationPolicies.*`; generated architecture tests assert no `RequireAuthorization("...")` magic-string usage. |
| No DB-backed identity or UI login flow | Pass | No database-backed user, role, login, cookie, Razor, Blazor, or identity-provider scaffolding was found in template production source. |
| Core remains lightweight | Pass | Core source has no JWT bearer reference, `AddJwtBearer`, `UseAuthentication`, `UseAuthorization`, `RequireAuthorization`, fake auth, or policy registration markers. |
| Core shared constants are inert | Pass | Core includes only BuildingBlocks authorization constants and generated `docs/authentication.md` states they are minimal shared names for future opt-in auth scaffolding. |
| Fake auth remains test-only | Pass | `FakeAuthentication*`, `Aegis.Test`, and `X-Test-*` markers are absent from generated pro/advanced production `src`; fake auth files live under generated integration test projects. |
| Fake auth supports permissions | Pass | Fake auth defaults include `X-Test-Permissions`; the handler emits `permission` and `scope` claims; helper clients include `AuthenticateWithPermissions`. |
| Permission integration tests | Pass | Generated `PermissionAuthorizationTests` include allowed and forbidden permission cases and use fake auth, not real JWT issuance. |
| Previous template behavior intact | Pass | Two full smoke runs built/tested core/pro/advanced, core and MediatR variants, AI/guardrail/hook/skill/docs/license options, P1B item templates, P1C architecture tests, and P1D-1 foundations. |
| Smoke quality and idempotency | Pass | `npm run template:smoke` passed twice in immediate fresh run directories and the latest generated-output directive scan returned `NO_UNRESOLVED_TEMPLATE_DIRECTIVES`. |
| Documentation and acceptance | Pass | `docs/authentication.md`, `docs/testing.md`, `docs/acceptance-criteria.md`, and `specs/0001-aegis-template-core/acceptance.md` accurately describe P1D-2A as scaffolded JWT validation plus claim-policy permissions, not a complete identity system. |
| OpenQuestions state | Pass | `OpenQuestions.md` contains `Q-20260609-004` with the inferred JWT bearer plus claim-policy default. No new blocker or inferred decision was found in this run. |

## Generated Output Evidence

Representative pro output from `mq6qu74l-43fe5481`:

- `g/pro-core/Directory.Packages.props`: JWT bearer package version.
- `g/pro-core/src/Smoke.ProCore.Api/Smoke.ProCore.Api.csproj`: JWT bearer package reference.
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisJwtOptions.cs`: `Authentication:Jwt` options.
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisJwtAuthenticationServiceCollectionExtensions.cs`: `AddJwtBearer`, signed-token validation, and reject-all fallback.
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisPermissionPolicyServiceCollectionExtensions.cs`: named claim-based policy registration.
- `g/pro-core/src/Smoke.ProCore.Api/Program.cs`: authentication middleware before authorization middleware.
- `g/pro-core/src/Smoke.ProCore.Api/Pro/ProProfileServices.cs`: protected operations endpoint.
- `g/pro-core/src/Smoke.ProCore.Modules/Modules/WorkItems/WorkItemsModule.cs`: protected starter module read/write endpoints.

Representative advanced output from `mq6qu74l-43fe5481`:

- `g/advanced-core/Directory.Packages.props`: JWT bearer package version.
- `g/advanced-core/src/Smoke.AdvancedCore.Api/Smoke.AdvancedCore.Api.csproj`: JWT bearer package reference.
- `g/advanced-core/src/Smoke.AdvancedCore.Api/Program.cs`: authentication middleware before authorization middleware.
- `g/advanced-core/src/Smoke.AdvancedCore.Api/Advanced/AdvancedProfileServices.cs`: protected advanced endpoint using `AegisAuthorizationPolicies.AdvancedRead`.
- `g/advanced-core/src/Smoke.AdvancedCore.Api/Pro/Auth/*`: same JWT and permission-policy scaffold as pro.

Representative core output from `mq6qu74l-43fe5481`:

- `g/core-core/src/Smoke.CoreCore.Api/Program.cs`: no auth middleware.
- `g/core-core/src/Smoke.CoreCore.Api/Smoke.CoreCore.Api.csproj`: no JWT bearer package reference.
- `g/core-core/src/Smoke.CoreCore.BuildingBlocks/Authorization/*`: shared permission, policy, and claim-type constants only.
- `g/core-core/docs/authentication.md`: documents core constants as minimal shared names and states core has no active JWT/auth/policy/fake-auth behavior.

## Fake Auth Isolation Evidence

Generated fake auth files are limited to integration test projects:

- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/Authentication/FakeAuthenticationHandler.cs`
- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/Authentication/FakeAuthenticationDefaults.cs`
- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/Authentication/FakeAuthenticationHeaders.cs`
- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/Infrastructure/AegisWebApplicationFactory.cs`
- Matching files under `g/advanced-core/tests/Smoke.AdvancedCore.IntegrationTests`.

Production source isolation checks:

- Search in generated pro/advanced `src` for `FakeAuthentication`, `Aegis.Test`, and `X-Test-*` returned no production markers.
- `AegisWebApplicationFactory.WithFakeAuthentication()` enables fake auth only in generated tests.
- Generated architecture tests assert production source does not reference fake auth infrastructure.

## Smoke Assertions Reviewed

`tools/guardrails/check.mjs` contains `assertP1D2AAuthPermissionSemantics`, which checks:

- pro/advanced JWT bearer package version and API project reference;
- typed JWT options and JWT registration;
- reject-all fallback and non-hardcoded random signing key for missing config;
- `UseAuthentication()` before `UseAuthorization()`;
- permission constants, policy constants, claim-type constants, and policy registration;
- protected pro, advanced, starter WorkItems, and TaskHub task endpoints;
- fake auth markers absent from production `Program.cs`;
- permission-aware fake auth defaults, handler, helper client, and allowed/forbidden tests;
- core excludes active JWT/auth/authorization/fake-auth wiring while keeping only shared constants.

`check.mjs` also runs generated-output directive scans. The latest generated-output scan returned:

```text
NO_UNRESOLVED_TEMPLATE_DIRECTIVES
```

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq6qmcxj-3e014ecf`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq6qu74l-43fe5481`. |
| `npm run check` after writing this report | Pass |

Generated smoke test observations:

- Core/core and core/MediatR architecture tests passed with 26 tests.
- Pro/advanced architecture tests passed with 26 tests.
- Pro/advanced integration tests passed 3 tests and skipped 1 Docker-gated Testcontainers test per generated variant.
- Mediator `core` and `mediatr` variants built and tested successfully.

## Documentation And Acceptance

Reviewed docs and acceptance state:

- `docs/authentication.md` describes JWT bearer validation and claim-based permissions, explicitly not an identity provider, token issuer, user database, role manager, or login UI.
- Generated `docs/authentication.md` matches generated behavior for core/pro/advanced profiles.
- `docs/testing.md` describes fake authentication as test-only and permission-aware.
- `docs/acceptance-criteria.md` marks P1D-2A as scaffolded auth/permissions and does not overstate it as complete identity management.
- `specs/0001-aegis-template-core/acceptance.md` marks P1D-2A criteria complete and scoped to scaffold semantics.
- `OpenQuestions.md` includes `Q-20260609-004` for the inferred JWT bearer plus claim-policy default.

## Validation Limitations

- Docker is not available on `PATH`, so live Testcontainers execution was not run.
- This is acceptable for P1D-2A because default smoke intentionally does not require Docker.
- No external identity provider was used or required.
- No real JWT issuance test was run; generated permission tests intentionally use test-only fake auth.
- This verification checked generated scaffold behavior and guardrail coverage, not production deployment hardening.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

No new true blockers or inferred decisions were identified.

Open blockers from `OpenQuestions.md`: none.

Relevant inferred decisions:

- `Q-20260609-004`: generate JWT bearer scaffolding for pro/advanced with user-supplied issuer/audience/signing key, reject missing config by default, use claim-based named policies, and keep fake auth only in integration tests.
- `Q-20260609-003`: fake authentication remains test-only.
- `Q-20260609-002`: generated Testcontainers tests remain opt-in.

## Remaining Gaps

Remaining work is outside P1D-2A:

- P1D-2B: inbox pattern.
- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Closure

P1D-2A remains confirmed closed.

Recommended next goal: start P1D-2B inbox pattern as a separate goal.
