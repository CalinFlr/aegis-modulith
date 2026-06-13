# P1D-1 Final Verification

Date: 2026-06-09

## Summary

P1D-1 is confirmed closed after the directive/template conditional cleanup.

This was a verification-only run. No fixes were implemented, and no P1D-2 JWT/auth/permissions/inbox work, P1D-3 contract/performance test work, P1D-4 deployment skeleton work, UI, event sourcing, public polish, screenshots, badges, docs site, or project rename work was started.

Reviewed baseline:

- Prior directive cleanup commit: `c36c9a3` (`test: close p1d directive verification gap`)
- Latest smoke run inspected: `artifacts/template-smoke/runs/mq6nmz5y-669b6bdd`
- Previous immediate smoke run: `artifacts/template-smoke/runs/mq6nekxg-f3427fb9`

## Pass/Fail Table

| Verification item | Result | Evidence |
| --- | --- | --- |
| Generated markdown/text/YAML docs have no unresolved template directives | Pass | `rg -n "^\s*(?:(?://|<!--)\s*)?#(?:if|else|elseif|elif|endif)\b" artifacts/template-smoke/runs/mq6nmz5y-669b6bdd -g "*.md" -g "*.txt" -g "*.yml" -g "*.yaml"` returned `NO_UNRESOLVED_TEMPLATE_DIRECTIVES`. |
| Generated core `docs/testing.md` is profile-accurate | Pass | `g/core-core/docs/testing.md` contains `## Core Profile`, describes architecture tests, and does not name `tests/Smoke.CoreCore.IntegrationTests` or include the pro/advanced integration-test section. |
| Generated pro `docs/testing.md` is profile-accurate | Pass | `g/pro-core/docs/testing.md` names `tests/Smoke.ProCore.IntegrationTests`, documents Testcontainers, fake authentication, and HttpClient resilience. |
| Generated advanced `docs/testing.md` is profile-accurate | Pass | `g/advanced-core/docs/testing.md` names `tests/Smoke.AdvancedCore.IntegrationTests`, documents Testcontainers, fake authentication, and HttpClient resilience. |
| Pro/advanced docs state Testcontainers live execution is opt-in and Docker-dependent | Pass | Pro and advanced docs say the Docker-backed test is skipped by default and show `AEGIS_RUN_TESTCONTAINERS=true` before running the containerized test. |
| Pro/advanced docs state fake auth is test-only | Pass | Pro and advanced docs say fake authentication is available only in the integration test project. |
| Core excludes Testcontainers, fake auth, and HttpClient resilience assets | Pass | `core-core` has no integration test project, no fake-auth handler, no `Pro/Http` resilience folder, no `Testcontainers.PostgreSql` package version, and no `Microsoft.Extensions.Http.Resilience` API project reference. |
| Pro/advanced include Testcontainers, fake auth, and HttpClient resilience assets | Pass | `pro-core` and `advanced-core` include integration test projects, `Testcontainers.PostgreSql`, `Microsoft.AspNetCore.Mvc.Testing`, fake-auth handlers, Docker-gated facts, and `AddStandardResilienceHandler` registrations. |
| Default smoke does not require Docker | Pass | Both default smoke runs passed while Docker was unavailable; generated container tests were skipped unless `AEGIS_RUN_TESTCONTAINERS=true` is set. |
| P0, P1A, P1B, and P1C behavior remains intact | Pass | `npm run check` passed, and two full `npm run template:smoke` runs passed across core/pro/advanced, mediator variants, AI/guardrail/docs/skills variants, item templates, and generated architecture tests. |

## Directive Evidence

Generated-output scan:

```powershell
rg -n "^\s*(?:(?://|<!--)\s*)?#(?:if|else|elseif|elif|endif)\b" artifacts/template-smoke/runs/mq6nmz5y-669b6bdd -g "*.md" -g "*.txt" -g "*.yml" -g "*.yaml"
```

Result:

```text
NO_UNRESOLVED_TEMPLATE_DIRECTIVES
```

`tools/guardrails/check.mjs` also contains `assertNoTemplateDirectives` and calls it for generated main-template outputs and item-template outputs.

## Profile-Accurate Testing Docs

Generated core docs:

- File: `artifacts/template-smoke/runs/mq6nmz5y-669b6bdd/g/core-core/docs/testing.md`
- Contains `## Core Profile`.
- States core includes architecture tests.
- States core does not include Testcontainers, fake authentication infrastructure, or pro HttpClient resilience files.
- Does not reference `tests/Smoke.CoreCore.IntegrationTests`.

Generated pro docs:

- File: `artifacts/template-smoke/runs/mq6nmz5y-669b6bdd/g/pro-core/docs/testing.md`
- References `tests/Smoke.ProCore.IntegrationTests`.
- States the Docker-backed test is skipped by default.
- Shows `AEGIS_RUN_TESTCONTAINERS=true` for opt-in Testcontainers execution.
- States fake authentication is only in the integration test project.
- Describes `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

Generated advanced docs:

- File: `artifacts/template-smoke/runs/mq6nmz5y-669b6bdd/g/advanced-core/docs/testing.md`
- References `tests/Smoke.AdvancedCore.IntegrationTests`.
- States the Docker-backed test is skipped by default.
- Shows `AEGIS_RUN_TESTCONTAINERS=true` for opt-in Testcontainers execution.
- States fake authentication is only in the integration test project.
- Describes `Microsoft.Extensions.Http.Resilience` and `AddStandardResilienceHandler`.

## Asset Evidence

Core exclusions:

- `g/core-core/Smoke.CoreCore.sln` does not include `IntegrationTests`.
- `g/core-core/tests/Smoke.CoreCore.IntegrationTests` does not exist.
- `g/core-core/tests/Smoke.CoreCore.IntegrationTests/Authentication/FakeAuthenticationHandler.cs` does not exist.
- `g/core-core/src/Smoke.CoreCore.Api/Pro/Http` does not exist.
- `g/core-core/Directory.Packages.props` contains neither `Testcontainers.PostgreSql` nor `Microsoft.Extensions.Http.Resilience`.
- `g/core-core/src/Smoke.CoreCore.Api/Smoke.CoreCore.Api.csproj` does not reference `Microsoft.Extensions.Http.Resilience`.

Pro/advanced inclusions:

- `g/pro-core/Smoke.ProCore.sln` includes `Smoke.ProCore.IntegrationTests`.
- `g/advanced-core/Smoke.AdvancedCore.sln` includes `Smoke.AdvancedCore.IntegrationTests`.
- Pro and advanced `Directory.Packages.props` include `Testcontainers.PostgreSql` and `Microsoft.AspNetCore.Mvc.Testing`.
- Pro and advanced integration test projects reference `Testcontainers.PostgreSql` and `Microsoft.AspNetCore.Mvc.Testing`.
- Pro and advanced include `Infrastructure/PostgresContainerFixture.cs`, `Infrastructure/AegisWebApplicationFactory.cs`, `Infrastructure/DockerFactAttribute.cs`, and `ContainerizedPostgresSmokeTests.cs`.
- Pro and advanced include `Authentication/FakeAuthenticationHandler.cs`, `FakeAuthenticationDefaults.cs`, `FakeAuthenticationHeaders.cs`, `TestUsers.cs`, `AuthenticatedClientExtensions.cs`, and `FakeAuthenticationSmokeTests.cs`.
- Pro and advanced APIs include `Pro/Http/ResilientHttpClientServiceCollectionExtensions.cs` and `SampleExternalStatusClient.cs`.
- Pro and advanced API projects reference `Microsoft.Extensions.Http.Resilience`.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass |
| `npm run template:smoke` | Pass; run directory `artifacts/template-smoke/runs/mq6nekxg-f3427fb9`. |
| `npm run template:smoke` again immediately | Pass; run directory `artifacts/template-smoke/runs/mq6nmz5y-669b6bdd`. |
| `npm run check` after writing this report | Pass |

Default smoke did not require Docker.

## Docker/Testcontainers Limitation

`Get-Command docker -ErrorAction SilentlyContinue` returned no Docker executable on `PATH`, so live Testcontainers execution was not run.

This is not a default validation failure. Generated Docker-backed tests are intentionally opt-in and guarded by `AEGIS_RUN_TESTCONTAINERS=true`.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

No new true blockers or inferred decisions were identified in this verification run.

Existing relevant inferred decisions remain:

- `Q-20260609-002`: generated Testcontainers tests stay opt-in.
- `Q-20260609-003`: fake authentication stays test-only until P1D-2.

Open blockers from `OpenQuestions.md`: none.

## Remaining Gaps

Remaining work is outside P1D-1:

- P1D-2: JWT/auth scaffolding, permissions hardening, and inbox pattern.
- P1D-3: contract tests and performance smoke tests.
- P1D-4: deployment skeleton.
- P2: public screenshots, badges, docs site, public polish, and release presentation.

## Closure

P1D-1 is confirmed closed.

Recommended next step: start P1D-2 as a separate goal.
