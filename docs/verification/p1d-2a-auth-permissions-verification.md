# P1D-2A Auth And Permissions Verification

Date: 2026-06-09

## Summary

P1D-2A is confirmed closed.

Generated `pro` and `advanced` outputs now include scaffolded JWT bearer authentication, named claim-based permission policies, protected example endpoints, permission-aware fake auth integration tests, architecture guardrails, and smoke assertions.

This implementation does not add inbox pattern work, P1D-3 contract/performance tests, P1D-4 deployment skeleton, UI, event sourcing, public polish, screenshots, badges, docs site work, or a project rename.

Clean validation runs:

- First clean smoke: `artifacts/template-smoke/runs/mq6pawk6-dcf754f9`
- Second clean smoke: `artifacts/template-smoke/runs/mq6pmrnk-aa34fdf0`

## Pass/Fail Table

| Verification item | Result | Evidence |
| --- | --- | --- |
| Pro/advanced include JWT bearer package references | Pass | Latest smoke output has `Microsoft.AspNetCore.Authentication.JwtBearer` in `Directory.Packages.props` and API `.csproj` for `pro-core` and `advanced-core`. |
| Pro/advanced include typed JWT options and registration | Pass | `src/<App>.Api/Pro/Auth/AegisJwtOptions.cs` and `AegisJwtAuthenticationServiceCollectionExtensions.cs` are generated and smoke asserts `Authentication:Jwt`, issuer, audience, signing key, `AddJwtBearer`, signed-token validation, and reject-all fallback. |
| Missing JWT config is safe by default | Pass | Generated JWT registration uses `CreateRejectAllValidationParameters` and `RandomNumberGenerator.GetBytes(64)` when issuer/audience/signing key are absent, so arbitrary tokens are not accepted. |
| Middleware order is correct | Pass | Generated pro/advanced `Program.cs` calls `app.UseAuthentication();` before `app.UseAuthorization();`; architecture tests and smoke assert the order. |
| Permission model is claim-based and named | Pass | Generated BuildingBlocks contains `AegisPermissions`, `AegisAuthorizationPolicies`, and `AegisPermissionClaimTypes`; policy registration checks `permission` and compatible `scope` claims. |
| Endpoint policy usage is demonstrated | Pass | Pro endpoint `/operations/outbox` requires `OperationsRead`; starter `WorkItems` endpoints require read/write policies; TaskHub `Tasks` endpoints require task read/write policies; advanced endpoint requires `AdvancedRead`. |
| Fake auth remains test-only | Pass | Fake auth files are only under generated integration tests; production `Program.cs` contains no fake auth scheme, `FakeAuthentication*`, `Aegis.Test`, or `X-Test-*` wiring. |
| Integration tests prove permission behavior | Pass | Generated integration tests include `PermissionAuthorizationTests` with one authorized request test and one forbidden request test. Latest pro/advanced integration test runs passed 3 tests and skipped 1 Docker-gated Testcontainers test. |
| Core remains lightweight | Pass | Core has no JWT bearer package/reference, no `Pro/Auth` folder, no auth middleware, no policy registration, no `RequireAuthorization` on starter endpoints, no integration test project, and no fake auth. It keeps only documented minimal shared permission constants in BuildingBlocks. |
| Mediator variants still build/test | Pass | Both clean smoke runs built and tested `pro-mediatr` and `advanced-mediatr` with the new auth/permission scaffold. |
| P0, P1A, P1B, P1C, and P1D-1 remain intact | Pass | `npm run check` passed; two clean full template smoke runs passed across profile, mediator, AI, guardrail, docs, license, and item-template coverage. |
| No unresolved template directives/tokens | Pass | Latest generated-output directive scan returned `NO_UNRESOLVED_TEMPLATE_DIRECTIVES`. |

## Evidence

Latest generated-output directive scan:

```powershell
rg -n '^\s*(?:(?://|<!--)\s*)?#(?:if|else|elseif|elif|endif)\b' artifacts/template-smoke/runs/mq6pmrnk-aa34fdf0 -g '*.md' -g '*.txt' -g '*.yml' -g '*.yaml' -g '*.cs' -g '*.csproj' -g '*.json'
```

Result:

```text
NO_UNRESOLVED_TEMPLATE_DIRECTIVES
```

Representative generated assets from `mq6pmrnk-aa34fdf0`:

- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisJwtOptions.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisJwtAuthenticationServiceCollectionExtensions.cs`
- `g/pro-core/src/Smoke.ProCore.Api/Pro/Auth/AegisPermissionPolicyServiceCollectionExtensions.cs`
- `g/pro-core/src/Smoke.ProCore.BuildingBlocks/Authorization/AegisPermissions.cs`
- `g/pro-core/src/Smoke.ProCore.BuildingBlocks/Authorization/AegisAuthorizationPolicies.cs`
- `g/pro-core/tests/Smoke.ProCore.IntegrationTests/PermissionAuthorizationTests.cs`
- `g/advanced-core/src/Smoke.AdvancedCore.Api/Advanced/AdvancedProfileServices.cs`

Representative generated behavior:

- `pro-core` and `advanced-core` API projects reference `Microsoft.AspNetCore.Authentication.JwtBearer`.
- `pro-core` and `advanced-core` `Program.cs` contain `UseAuthentication` before `UseAuthorization`.
- `pro-core` and `advanced-core` fake auth defaults include `X-Test-Permissions`.
- `pro-core` and `advanced-core` production `Program.cs` do not contain fake auth markers.
- `core-core` has no pro auth folder, JWT bearer reference, auth middleware, policy registration, integration tests, or fake auth.

## Smoke Assertions Added

`tools/guardrails/check.mjs` now asserts P1D-2A semantics:

- pro/advanced JWT bearer package and API references;
- typed JWT options and reject-all fallback for missing config;
- `UseAuthentication` before `UseAuthorization`;
- permission constants, claim-type constants, policy constants, and policy registration;
- policy usage on pro, advanced, starter module, and TaskHub task endpoints;
- fake auth stays test-only and production `Program.cs` does not wire it;
- integration tests include permission-aware fake auth helpers and allowed/forbidden permission tests;
- core excludes pro/advanced JWT/auth/policy/fake-auth assets except documented minimal shared permission constants.

Generated architecture tests also assert:

- auth middleware order;
- production projects do not reference fake auth infrastructure;
- endpoint authorization uses named policy constants instead of magic strings;
- auth source/configuration does not contain hardcoded production signing-key strings.

## Checks Run

| Command | Result | Evidence |
| --- | --- | --- |
| Targeted generated pro build after JWT scaffold | Pass | `artifacts/p1d-2a-jwt-ca977beb`; generated pro solution built with 0 warnings/errors. |
| Targeted generated pro and TaskHub builds after permission scaffold | Pass | `artifacts/p1d-2a-permissions-177edd90`; both generated solutions built with 0 warnings/errors. |
| Targeted generated pro integration tests after fake auth update | Pass for integration tests | `artifacts/p1d-2a-fakeauth-9ad3963c`; integration tests passed 3 and skipped 1 Docker-gated test. Architecture tests still expected the old pro-service call and were updated in the next commit. |
| Targeted generated pro solution after architecture update | Pass | `artifacts/p1d-2a-arch-a759c2ee`; architecture tests passed 26, integration tests passed 3 and skipped 1. |
| `npm run check` | Pass | Repository guardrails passed. |
| First full `npm run template:smoke` attempt | Fail, then fixed | `artifacts/template-smoke/runs/mq6ou4oq-c8c8fbc1`; generated builds/tests passed, but new smoke assertions expected one-line `RequireAuthorization(...)` formatting. The assertion was corrected to check named policy constants without depending on line wrapping. |
| Restarted `npm run check` | Pass | Repository guardrails passed after the smoke assertion fix. |
| `npm run template:smoke` | Pass | Clean run directory `artifacts/template-smoke/runs/mq6pawk6-dcf754f9`. |
| `npm run template:smoke` again immediately | Pass | Clean run directory `artifacts/template-smoke/runs/mq6pmrnk-aa34fdf0`. |

## Docker/Testcontainers Limitation

`Get-Command docker -ErrorAction SilentlyContinue` returned no Docker executable on `PATH`, so live Testcontainers execution was not run.

This is not a default validation failure. Generated Docker-backed tests remain opt-in and skipped unless `AEGIS_RUN_TESTCONTAINERS=true` is set.

## OpenQuestions.md Updates

Added one inferred decision:

- `Q-20260609-004`: generated auth uses JWT bearer scaffolding with user-supplied issuer/audience/signing key, claim-based named permission policies, reject-all missing-config behavior, and test-only fake auth.

Open blockers from `OpenQuestions.md`: none.

## Remaining Gaps

Remaining work is outside P1D-2A:

- P1D-2B: inbox pattern.
- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Closure

P1D-2A is confirmed closed.

Recommended next goal: start P1D-2B inbox pattern as a separate goal.
