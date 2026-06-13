# P1C Architecture Tests Verification

Date: 2026-06-09

## Summary

P1C is confirmed closed.

Generated architecture tests now validate architecture rules beyond buildability and basic file shape. The generated suite runs in every main smoke variant and currently passes 23 architecture tests per generated solution.

No P1D pro/advanced feature-depth work was started. No UI, event sourcing, public screenshots, badges, docs site work, P2 public polish, project rename, or architecture redesign was added.

## What Changed

- Added focused architecture rule docs:
  - `docs/architecture/vertical-slices.md`
  - `docs/architecture/cqrs-lite.md`
- Linked those docs from `docs/architecture.md`.
- Added root docs guardrail coverage for the new architecture docs.
- Expanded generated architecture tests under `tests/<App>.ArchitectureTests`.
- Added smoke assertions that generated architecture tests exist, are included in solutions, run through `dotnet test`, contain the expected rule coverage, and have no unresolved template tokens.
- Updated acceptance docs for P1C.

## Generated Architecture Rules Now Tested

Module boundaries:

- Project references do not point to Infrastructure projects.
- Modules do not reference another module's Infrastructure namespace.
- Cross-module Contracts references must be declared in `module.json`.
- Manifest dependencies default to known module Contracts.
- Generated code does not use generic repositories.

Module manifests:

- Every module has `module.json`.
- Required manifest fields exist.
- Manifest name matches the module folder.
- `type` is `business-module`.
- Schema is present.
- `allowCrossModuleDatabaseAccess` is `false`.
- `allowInfrastructureReferences` is `false`.
- Listed public contracts exist under the module Contracts folder.

Domain isolation:

- Domain source files do not directly use ASP.NET Core, EF Core, Npgsql, or Infrastructure namespaces.
- Domain source files do not reference another module's Domain namespace.
- Domain events are module-owned and derive from `DomainEvent`.
- Domain event files live under a module `Events` path.

CQRS and vertical slices:

- Commands implement `ICommand<TResponse>`.
- Queries implement `IQuery<TResponse>`.
- Command handlers implement `ICommandHandler<TCommand, TResponse>`.
- Query handlers implement `IQueryHandler<TQuery, TResponse>`.
- MediatR variants also implement MediatR request/request-handler abstractions.
- Query handlers do not call `SaveChanges` or `SaveChangesAsync`.
- EF-backed query handlers use `AsNoTracking()`.
- Command/query slice files live under `Features/<SliceName>`.

API/DTO separation:

- Module endpoint mappings do not perform direct persistence.
- Module endpoint mappings delegate feature requests to command/query dispatchers.
- Module endpoint mappings use `Results.*` HTTP mapping.
- Endpoint mapping bodies do not reference Domain namespaces.

Profile and option wiring:

- Core profile excludes AppHost, ServiceDefaults, Dockerfile, pro wiring, and advanced wiring.
- Pro/advanced profiles include AppHost, ServiceDefaults, Dockerfile, pro services, rate limiting, and pro endpoints.
- Advanced profile includes advanced services and endpoints.
- Core mediator uses core handler registration and service-provider dispatchers.
- MediatR mode uses active MediatR registration and dispatchers.

Persistence:

- Each generated module has one module-scoped DbContext.
- DbContext namespace is module Infrastructure-scoped.
- DbContext default schema matches `module.json`.
- Generated DbContexts do not configure foreign keys by default.
- Domain and persistence sources do not reference another module's Domain or Infrastructure namespaces.

## Smoke Assertions Added

`npm run template:smoke` now asserts for every main generated variant:

- Architecture test project exists.
- Architecture test project is included in the generated solution.
- Architecture test project references generated BuildingBlocks and Modules projects.
- Architecture rule test files exist.
- Generated architecture tests contain no unresolved template tokens.
- Manifest boundary-rule assertions are present.
- Module-boundary assertions are present.
- CQRS/query/MediatR assertions are present.
- Profile/core/advanced/MediatR/core-dispatcher assertions are present.
- Domain isolation and domain-event assertions are present.
- Endpoint persistence assertions are present.
- DbContext/schema/FK assertions are present.

Because the smoke runner executes `dotnet test <solution> -c Release --no-build` for each generated solution, these architecture tests run in the smoke matrix.

## Validation

Final required validation:

| Command | Result | Evidence |
| --- | --- | --- |
| `npm run check` | Pass | `ai instructions`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, and `security` passed. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq6c1jnk-1f113c32`. |
| `npm run template:smoke` again immediately | Pass | Run directory: `artifacts/template-smoke/runs/mq6c7k5h-e0078c10`. |

Earlier targeted probes also passed for:

- `core + core mediator`
- `pro + MediatR`
- `pro + TaskHub sample`

Each generated architecture test run in the final smoke matrix reported 23 passing tests for the generated architecture test assembly.

## OpenQuestions.md

`OpenQuestions.md` was not changed.

No true blockers or new inferred decisions were found.

Existing inferred decisions remain:

- `Q-20260606-001`: Confirm final public project name before publish.
- `Q-20260606-002`: Adopt spec-driven development layer.
- `Q-20260606-003`: Include `module.json` manifests in generated modules.
- `Q-20260606-004`: Pin optional MediatR to Apache-2.0 package line.
- `Q-20260608-001`: Define the generated core AI skill subset.
- `Q-20260609-001`: Prefer drop-in item templates over automatic solution mutation.

No blockers are listed in `OpenQuestions.md`.

## Validation Limitations

- Smoke builds and tests generated solutions but does not run generated APIs or exercise HTTP endpoints.
- Domain isolation is validated at source-file and namespace-usage level because the current generated Modules project also contains endpoint and infrastructure code, so assembly-level references include ASP.NET Core and EF Core.
- Cross-module foreign keys are validated by absence of generated FK configuration and cross-module domain/infrastructure namespace usage; scalar IDs such as `ProjectId` remain allowed.
- Endpoint DTO separation is validated by endpoint mapping source conventions and absence of Domain namespace usage in endpoint mapping bodies, not by runtime HTTP contract inspection.
- Tests do not attempt to prove business intent quality of command names beyond the generated CQRS abstraction and vertical-slice placement.

## Remaining Gaps

Remaining work is outside P1C:

- P1D pro/advanced feature-depth work.
- P2 public polish, screenshots, badges, docs site, and release presentation.
- Runtime API endpoint exercise beyond generated build/test.
- Broader public documentation polish.

Event sourcing remains out of scope and was not introduced.

## Closure

P1C is confirmed closed.

Recommended next step: start P1D pro/advanced feature-depth work as a separate goal.
