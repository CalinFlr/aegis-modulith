# P0 Post-Fix Verification

Date: 2026-06-08

## Summary

P0 is improved but not fully closed.

The post-review commits made `--mediator mediatr` semantically active and restored runtime wiring for `pro` and `advanced` profiles in generated `Program.cs`. Supplemental generation, restore, build, and test validation of the full eight-variant matrix passed under `artifacts/p0-post-fix-verification`.

Two P0 closure issues remain:

- `npm run template:smoke` does not currently complete in this workspace. It failed twice during `pro-mediatr` generation because stale or locked output under `artifacts/template-smoke/generated/pro-mediatr` caused `dotnet new` to require `--force`.
- Generated `core` output excludes AppHost, ServiceDefaults, Dockerfile, and solution references, but `src/<Name>.Api/<Name>.Api.csproj` still contains a conditional `ProjectReference` to the absent ServiceDefaults project. The current docs do not explicitly allow this leftover reference.

## Commits Reviewed After Previous Verification

Previous verification report commit:

- `9b5cb2c` - `docs: add current implementation verification report`

Commits after that report:

| Commit | Subject | Relevant effect |
| --- | --- | --- |
| `9c12409` | `fix: activate profile template wiring` | Changed template profile conditions, `Program.cs`, solution entries, and AppHost wiring. |
| `bb84e11` | `fix: activate mediatr template dispatching` | Changed CQRS dispatcher and feature request/handler templates for MediatR mode. |
| `eb46c22` | `test: assert template option semantics` | Added semantic smoke assertions for mediator, profiles, options, hooks, and sample shape. |
| `70b98f8` | `test: make template smoke cleanup resilient` | Added retry/fallback cleanup behavior for smoke artifacts. |

## P0 Pass/Fail Table

| P0 area | Result | Evidence | Notes |
| --- | --- | --- | --- |
| `--mediator mediatr` is semantically active | Pass | Generated `core-mediatr`, `pro-mediatr`, and `advanced-mediatr` variants include `services.AddMediatR`, `MediatRCommandDispatcher`, `MediatRQueryDispatcher`, and `ISender sender`; generated commands implement `MediatR.IRequest<T>` and handlers implement `MediatR.IRequestHandler<TRequest,TResponse>`. | Supplemental generated matrix built and tested successfully. |
| `pro` and `advanced` profiles are semantically active | Pass | Generated `pro-core` `Program.cs` calls `builder.AddServiceDefaults()`, `builder.Services.AddProProfileServices()`, `app.UseRateLimiter()`, and `app.MapProProfileEndpoints()`. Generated `advanced-core` also calls `builder.Services.AddAdvancedProfileServices()` and `app.MapAdvancedProfileEndpoints()`. | Supplemental generated matrix built and tested successfully. |
| Core/pro/advanced file-shape drift is resolved | Fail | Generated `core-core` has no AppHost project, no ServiceDefaults project, no Dockerfile, and no solution references to AppHost or ServiceDefaults. However, generated `core-core/src/Verify.CoreCore.Api/Verify.CoreCore.Api.csproj` still contains `<ProjectReference Include="..\Verify.CoreCore.ServiceDefaults\Verify.CoreCore.ServiceDefaults.csproj" Condition="'$(AegisProfile)' != 'core'" />`. | The reference is inactive because `Directory.Build.props` sets `<AegisProfile>core</AegisProfile>`, but the requested verification included "related project references." Docs still place AppHost/ServiceDefaults/Dockerfile under `pro`, not `core`. |
| Template smoke tests include semantic assertions | Fail | `tools/guardrails/check.mjs` now asserts MediatR dispatching, core-vs-MediatR request/handler shape, pro/advanced `Program.cs` wiring, core exclusions, option values, hooks, and sample module shape. | The smoke command itself failed before completing, and its core exclusion assertions do not check the generated API csproj for stale conditional ServiceDefaults references. |

## Exact Generated Evidence

Generated outputs were inspected under `artifacts/p0-post-fix-verification`, produced from the package built by the smoke run.

### MediatR Mode

`artifacts/p0-post-fix-verification/core-mediatr/src/Verify.CoreMediatR.BuildingBlocks/Cqrs/DispatchingServiceCollectionExtensions.cs` contains:

```csharp
services.AddMediatR(configuration =>
{
    foreach (var assembly in handlerAssemblies)
    {
        configuration.RegisterServicesFromAssembly(assembly);
    }
});
services.AddScoped<ICommandDispatcher, MediatRCommandDispatcher>();
services.AddScoped<IQueryDispatcher, MediatRQueryDispatcher>();
```

The same file contains MediatR-backed dispatchers:

```csharp
private sealed class MediatRCommandDispatcher(ISender sender) : ICommandDispatcher
private sealed class MediatRQueryDispatcher(ISender sender) : IQueryDispatcher
```

`artifacts/p0-post-fix-verification/core-mediatr/src/Verify.CoreMediatR.Modules/Modules/WorkItems/Features/CreateWorkItem/CreateWorkItemCommand.cs` contains:

```csharp
public sealed record CreateWorkItemCommand(string Title) : ICommand<CreateWorkItemResponse>
    , MediatR.IRequest<CreateWorkItemResponse>
;
```

`artifacts/p0-post-fix-verification/core-mediatr/src/Verify.CoreMediatR.Modules/Modules/WorkItems/Features/CreateWorkItem/CreateWorkItemHandler.cs` contains:

```csharp
public sealed class CreateWorkItemHandler(WorkItemsDbContext dbContext) :
    ICommandHandler<CreateWorkItemCommand, CreateWorkItemResponse>
    , MediatR.IRequestHandler<CreateWorkItemCommand, CreateWorkItemResponse>
```

`rg` confirmed the same MediatR dispatcher/request/handler patterns in `pro-mediatr` and `advanced-mediatr`. It also confirmed `RegisterCoreHandlers` and `ServiceProviderCommandDispatcher` appear in core mediator variants, not the MediatR variants.

### Pro And Advanced Wiring

`artifacts/p0-post-fix-verification/pro-core/src/Verify.ProCore.Api/Program.cs` contains:

```csharp
builder.AddServiceDefaults();
builder.Services.AddProProfileServices();
app.UseRateLimiter();
app.MapProProfileEndpoints();
```

`artifacts/p0-post-fix-verification/advanced-core/src/Verify.AdvancedCore.Api/Program.cs` contains:

```csharp
builder.AddServiceDefaults();
builder.Services.AddProProfileServices();
builder.Services.AddAdvancedProfileServices();
app.UseRateLimiter();
app.MapProProfileEndpoints();
app.MapAdvancedProfileEndpoints();
```

`artifacts/p0-post-fix-verification/pro-core/src/Verify.ProCore.Api/Advanced/AdvancedProfileServices.cs` is absent. `artifacts/p0-post-fix-verification/advanced-core/src/Verify.AdvancedCore.Api/Advanced/AdvancedProfileServices.cs` is present.

### Core File Shape

Generated `core-core` output:

- `artifacts/p0-post-fix-verification/core-core/src/Verify.CoreCore.AppHost/Verify.CoreCore.AppHost.csproj`: absent.
- `artifacts/p0-post-fix-verification/core-core/src/Verify.CoreCore.ServiceDefaults/Verify.CoreCore.ServiceDefaults.csproj`: absent.
- `artifacts/p0-post-fix-verification/core-core/Dockerfile`: absent.
- `artifacts/p0-post-fix-verification/core-core/Verify.CoreCore.sln`: contains only Api, BuildingBlocks, Modules, and ArchitectureTests projects.

Remaining drift:

```xml
<ProjectReference Include="..\Verify.CoreCore.ServiceDefaults\Verify.CoreCore.ServiceDefaults.csproj" Condition="'$(AegisProfile)' != 'core'" />
```

This appears in `artifacts/p0-post-fix-verification/core-core/src/Verify.CoreCore.Api/Verify.CoreCore.Api.csproj`.

### Smoke Assertions

`tools/guardrails/check.mjs` now includes semantic assertions for:

- selected option values in `Directory.Build.props`: `AegisProfile`, `AegisMediator`, `AegisSample`, and `AegisGuardrails`;
- MediatR mode: `services.AddMediatR`, `MediatRCommandDispatcher`, `MediatRQueryDispatcher`, `ISender sender`, no `RegisterCoreHandlers(services`, no `ServiceProviderCommandDispatcher`, command `MediatR.IRequest<T>`, and handler `MediatR.IRequestHandler<TRequest,TResponse>`;
- core mediator mode: core handler registration and no MediatR request/handler shape;
- profile mode: AppHost, ServiceDefaults, Dockerfile, pro services/endpoints, advanced services/endpoints, and rate limiter wiring;
- hooks: `lefthook.yml` present only for `hooks=lefthook`;
- sample shape: TaskHub has Projects, Tasks, Notifications, and Audit module manifests and excludes starter WorkItems; `sample=none` keeps WorkItems and excludes Projects.

Assertion coverage gap:

- The smoke runner does not assert that generated core API csproj files exclude stale conditional pro-only project references.

## Checks Run

| Command | Result | Notes |
| --- | --- | --- |
| `npm run check` | Pass | Passed `ai instructions`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, and `security`. |
| `npm run template:smoke` | Fail | Failed during `pro-mediatr` generation with `Creating this template will make changes to existing files` and `Generation failed for pro-mediatr.` |
| `npm run template:smoke` retry | Fail | Failed at the same `pro-mediatr` generation point. |
| `dotnet build-server shutdown` | Pass | Used to try to release generated artifact locks before retrying smoke. |
| Supplemental manual matrix generation under `artifacts/p0-post-fix-verification` | Pass | Generated `core-core`, `core-mediatr`, `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `taskhub`, and `strict-enterprise`. |
| Supplemental manual restore/build/test for generated matrix | Pass | All eight generated solutions restored, built in Release with 0 warnings and 0 errors, and passed 4 architecture tests each. |

## Remaining P1/P2 Gaps

- Generated enterprise AI assets are still intentionally out of scope for this run and remain a lower-priority gap if the public CLI docs continue to imply generated `.ai`, `.agents`, guardrail runner, workflows, policies, skills, and eval assets.
- Item templates remain outside this P0 verification. The previous concerns about slice/event/module depth and integration with the generated architecture are not revalidated here.
- Public polish remains out of scope: screenshots, badges, docs site, UI, and public-facing polish checklist items were not added or verified.
- Advanced/pro feature depth remains out of scope: auth/JWT scaffolding, inbox pattern, contract/performance tests, deployment skeleton, Testcontainers depth, fake auth test handler, and resilience defaults were not revalidated beyond the existing generated build/test matrix.
- Architecture test coverage still does not prove every non-negotiable architecture rule, such as query non-mutation, DTO/entity separation, no cross-module foreign keys, or no cross-module EF navigation properties.
- Event sourcing remains correctly out of scope and was not added or verified.

## Validation Limitations

- The required `npm run template:smoke` command failed twice, so CI-equivalent smoke validation is not green.
- The smoke failure was caused by generated output conflicts under `artifacts/template-smoke`, apparently related to locked stale artifacts. A direct PowerShell cleanup attempt also failed to remove locked generated directories.
- Supplemental manual generation used a separate ignored output root and its own `DOTNET_CLI_HOME`; it validates generated behavior but does not replace the failing repository smoke command.
- The supplemental checks inspect and build generated outputs; they do not exercise HTTP endpoints at runtime.
- The smoke assertions validate important semantics but still miss the stale conditional ServiceDefaults `ProjectReference` in generated core API csproj output.

## Supersession Of Current Implementation Review

`docs/verification/current-implementation-review.md` is partially superseded for P0 areas:

- Superseded for MediatR semantics: generated MediatR variants now contain active MediatR registration, dispatchers, and request/handler integration.
- Superseded for pro/advanced runtime wiring: generated `Program.cs` now wires pro and advanced services/endpoints as intended.
- Not fully superseded for core file-shape drift: the major pro-only files and solution references are gone from core, but an inactive conditional ServiceDefaults project reference remains in the core API csproj.
- Not superseded for smoke validation: semantic assertions were added, but the official `npm run template:smoke` command currently fails before completing and misses the remaining core project-reference drift.

## OpenQuestions.md

`OpenQuestions.md` was not changed.

No true human-decision blocker or new inferred decision was found. The remaining findings are concrete implementation and validation gaps with clear fix paths.

## P0 Closure

P0 is not confirmed closed.

Recommended next goal: fix the smoke artifact cleanup/generation failure and add a semantic smoke assertion that generated core API projects do not contain pro-only AppHost/ServiceDefaults/Dockerfile-related project references.
