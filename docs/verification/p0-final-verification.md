# P0 Final Verification

Date: 2026-06-08

## Summary

P0 is confirmed closed for the remaining issues from `docs/verification/p0-post-fix-verification.md`.

The generated `core` profile no longer contains inactive or conditional references to `ServiceDefaults`, `AppHost`, or `Dockerfile` assets. The smoke runner now generates each run under a unique directory and no longer relies on deleting stale or locked output as a prerequisite for success.

## Fixes Verified

### Core file shape

The API project template now uses template-time XML conditional processing for the `ServiceDefaults` project reference instead of leaving an MSBuild condition in generated output.

Generated `core-core` evidence from:

```text
artifacts/template-smoke/runs/mq55tqlr-7a7803b0/g/core-core
```

Observed results:

| Check | Result |
| --- | --- |
| `src/Smoke.CoreCore.ServiceDefaults/Smoke.CoreCore.ServiceDefaults.csproj` exists | `False` |
| `src/Smoke.CoreCore.AppHost/Smoke.CoreCore.AppHost.csproj` exists | `False` |
| `Dockerfile` exists | `False` |
| `src/Smoke.CoreCore.Api/Smoke.CoreCore.Api.csproj` contains `ServiceDefaults` | `0` matches |
| `Smoke.CoreCore.sln` contains `ServiceDefaults` or `AppHost` | `0` matches |

The smoke assertions now fail `core` output if:

- a `ServiceDefaults` project exists;
- an `AppHost` project exists;
- a `Dockerfile` exists;
- the API project contains `ServiceDefaults`;
- the solution contains `ServiceDefaults` or `AppHost`.

### Smoke idempotency

`npm run template:smoke` now creates a fresh run directory:

```text
artifacts/template-smoke/runs/<short-timestamp>-<short-guid>
```

Each run has isolated package output, generated output, item output, `DOTNET_CLI_HOME`, and `NUGET_PACKAGES`. `artifacts/template-smoke/latest-run.txt` points to the latest run for inspection. The runner does not delete previous generated outputs before starting.

Back-to-back proof:

| Run | Result | Run directory |
| --- | --- | --- |
| First required smoke run | Pass | `artifacts/template-smoke/runs/mq55gpx8-6aab18fe` |
| Second required smoke run immediately after | Pass | `artifacts/template-smoke/runs/mq55tqlr-7a7803b0` |

Both runs packed, installed, generated, restored, built, and tested the smoke matrix and item templates. Every generated solution build completed with `0 Warning(s)` and `0 Error(s)`, and each generated architecture test run passed `4` tests.

## Checks Run

| Command | Result |
| --- | --- |
| `npm run check` | Pass |
| `npm run template:smoke` | Pass |
| `npm run template:smoke` again immediately | Pass |

Additional targeted verification before the first fix commit generated `core` and `pro` outputs in a unique artifact directory and confirmed that `core` had no `ServiceDefaults`/`AppHost`/`Dockerfile` output or references while `pro` still had the `ServiceDefaults` API project reference.

## Remaining P1/P2 Gaps

- Generated enterprise AI assets remain intentionally out of scope.
- Item template polish remains intentionally out of scope.
- Public polish, screenshots, badges, docs site, UI, and visual work remain out of scope.
- Advanced/pro feature depth remains a lower-priority gap beyond the validated buildable scaffolding.
- Architecture tests still do not prove every documented architecture rule, such as query non-mutation, DTO/entity separation, and absence of cross-module EF navigation properties.
- Event sourcing was not added and remains correctly out of scope.

## Validation Limitations

- Smoke validation builds and tests generated solutions and asserts important generated file semantics; it does not run generated APIs and exercise HTTP endpoints.
- The idempotency proof validates repeated runs by avoiding stale output collisions, not by proving every possible locked-file behavior in old artifact folders.
- P1 and P2 areas were not fixed or revalidated beyond the checks that already run as part of `npm run check` and `npm run template:smoke`.

## OpenQuestions.md

`OpenQuestions.md` was not changed.

No true blocker or new inferred decision was found. Existing inferred decisions remain as previously recorded.

## P0 Closure

P0 is confirmed closed.

Recommended next goal: start P1/P2 triage separately, beginning with the documented generated enterprise AI asset scope and item template quality gaps.
