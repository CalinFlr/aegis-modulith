# P1C Post-Fix Verification

Date: 2026-06-09

## Summary

P1C remains confirmed closed after re-verifying the architecture-test expansion.

This run verified only P1C. No fixes were implemented. P1D pro/advanced feature-depth work, UI, event sourcing, public screenshots, badges, docs site work, P2 public polish, and project renaming were not started.

Reviewed P1C commit sequence:

- `87b7b77`: added focused CQRS-lite and vertical-slice architecture docs and docs guardrail coverage.
- `f39097f`: expanded generated architecture tests across module boundaries, manifests, Domain isolation, CQRS, endpoints, persistence, and profile/mediator wiring.
- `6329afc`: added smoke assertions for architecture-test coverage markers and unresolved architecture-test template tokens.
- `7c05f2e`: added the first P1C verification report and acceptance updates.

## P1C Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| Architecture documentation alignment | Pass | `docs/architecture/cqrs-lite.md` and `docs/architecture/vertical-slices.md` exist, are linked from `docs/architecture.md`, and describe the generated CQRS-lite and vertical-slice behavior without introducing event sourcing or separate read/write databases. |
| Module boundary tests | Pass | `ModuleBoundaryTests.cs` parses project references, checks cross-module Infrastructure namespace usage, requires cross-module Contracts references to be declared in `module.json`, rejects non-Contracts manifest dependencies, rejects generic repository markers, and verifies CQRS handlers are discoverable. |
| Module manifest tests | Pass | `ModuleManifestTests.cs` parses `module.json`, validates required fields, module name, `business-module` type, non-empty schema, default boundary-rule booleans, and listed public contract files. |
| Domain isolation tests | Pass | `DomainIsolationTests.cs` checks Domain source files for ASP.NET Core, EF Core, Npgsql, and Infrastructure markers; checks cross-module Domain namespace usage in Domain files; and verifies module-owned `DomainEvent` types. |
| CQRS and MediatR tests | Pass | `CqrsArchitectureTests.cs` uses reflection to verify command/query interfaces, handler bindings, optional MediatR request/handler compatibility, query non-mutation, EF `AsNoTracking()`, and `Features/<SliceName>` placement. |
| Endpoint tests | Pass | `ApiEndpointTests.cs` parses module `MapEndpoints` bodies and asserts no direct persistence calls, dispatcher delegation, `Results.*` mapping, and no Domain namespace usage in endpoint bodies. |
| Profile and mediator wiring tests | Pass | `ProfileOptionWiringTests.cs` reads `Directory.Build.props` and checks core/pro/advanced file and `Program.cs` wiring plus core dispatcher vs MediatR dispatcher registration. |
| Persistence tests | Pass | `PersistenceArchitectureTests.cs` checks one module-scoped DbContext per module, Infrastructure namespace placement, schema alignment with `module.json`, no generated FK configuration markers, and no cross-module Domain/Infrastructure namespace usage in Domain/Infrastructure sources. |
| Smoke architecture assertions | Pass | `tools/guardrails/check.mjs` asserts architecture test project inclusion, expected architecture test files, BuildingBlocks/Modules project references, coverage markers, and absence of unresolved architecture-test template tokens. |
| Smoke idempotency | Pass | Two immediate `npm run template:smoke` runs passed in separate fresh run directories. |

Architecture docs do not overpromise P1C validation. They state the generated architecture rules and current generated shapes. The only inherently qualitative rule is that commands express business intent; generated examples use intent-shaped names such as create/list/get use cases, while tests verify the CQRS abstraction and handler binding rather than trying to prove naming quality.

## Generated Architecture Test Evidence

Latest generated smoke output:

```text
artifacts/template-smoke/runs/mq6hmydm-b19ff281
```

Every generated variant in that run included an architecture test project in the solution and 23 `[Fact]` architecture tests:

| Variant | Architecture test project | Facts | In solution |
| --- | --- | ---: | --- |
| `advanced-core` | `Smoke.AdvancedCore.ArchitectureTests` | 23 | Yes |
| `advanced-mediatr` | `Smoke.AdvancedMediatR.ArchitectureTests` | 23 | Yes |
| `ai-agents` | `Smoke.AiAgents.ArchitectureTests` | 23 | Yes |
| `ai-none` | `Smoke.AiNone.ArchitectureTests` | 23 | Yes |
| `core-core` | `Smoke.CoreCore.ArchitectureTests` | 23 | Yes |
| `core-mediatr` | `Smoke.CoreMediatR.ArchitectureTests` | 23 | Yes |
| `guardrails-off-lefthook` | `Smoke.GuardrailsOff.ArchitectureTests` | 23 | Yes |
| `pro-core` | `Smoke.ProCore.ArchitectureTests` | 23 | Yes |
| `pro-mediatr` | `Smoke.ProMediatR.ArchitectureTests` | 23 | Yes |
| `skills-core-license-mit` | `Smoke.SkillsCoreLicenseMit.ArchitectureTests` | 23 | Yes |
| `skills-none-docs-standard` | `Smoke.SkillsNoneDocsStandard.ArchitectureTests` | 23 | Yes |
| `strict-enterprise` | `Smoke.StrictEnterprise.ArchitectureTests` | 23 | Yes |
| `taskhub` | `Aegis.TaskHub.ArchitectureTests` | 23 | Yes |

Representative generated `pro-mediatr` test methods:

- `Project_references_do_not_point_to_infrastructure_projects`
- `Modules_do_not_reference_another_modules_infrastructure_namespace`
- `Cross_module_contract_references_are_declared_in_module_manifests`
- `Every_module_has_a_valid_manifest_with_boundary_rules`
- `Domain_source_files_do_not_depend_on_web_or_persistence_infrastructure`
- `Commands_and_queries_follow_generated_abstractions`
- `Command_and_query_handlers_are_bound_to_their_requests`
- `Query_handlers_do_not_mutate_state_and_use_no_tracking_for_ef_queries`
- `Command_and_query_slice_files_live_under_feature_folders`
- `Module_endpoint_mappings_do_not_perform_persistence_directly`
- `Module_endpoint_mappings_delegate_feature_requests_to_dispatchers`
- `Profile_wiring_matches_selected_profile`
- `Mediator_wiring_matches_selected_mediator`
- `Each_module_has_one_module_scoped_dbcontext`
- `Dbcontext_default_schemas_match_module_manifests`
- `Generated_dbcontexts_do_not_configure_foreign_keys_by_default`

The latest generated architecture-test token scan found:

```text
No unresolved architecture-test template tokens found in latest generated smoke output.
```

## Smoke Output Evidence

Required validation passed:

| Command | Result | Evidence |
| --- | --- | --- |
| `npm run check` | Pass | `ai instructions`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, and `security` passed. Rerun after adding this report also passed. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq6hgw69-b9674c3b`. Visible generated architecture test runs reported `Passed: 23`. |
| `npm run template:smoke` again immediately | Pass | Run directory: `artifacts/template-smoke/runs/mq6hmydm-b19ff281`. Visible generated architecture test runs reported `Passed: 23`. |

Smoke uses fresh run directories under `artifacts/template-smoke/runs/<run-id>`, isolates `DOTNET_CLI_HOME` and `NUGET_PACKAGES`, writes `latest-run.txt`, and does not depend on stale generated outputs.

## Maintainability Assessment

The generated architecture tests are meaningful and not just brittle word checks.

Strong points:

- Reflection verifies generated command/query request interfaces, handler bindings, MediatR request/handler compatibility, CQRS handler discoverability, and domain event inheritance.
- JSON parsing verifies manifests and manifest arrays rather than only checking for text.
- XML parsing verifies project references rather than grepping `.csproj` files blindly.
- Filesystem checks verify generated module folders, module-scoped DbContexts, and solution inclusion.
- Regex validates `HasDefaultSchema("<manifest schema>")` alignment.
- Source-body checks are scoped to specific generated method bodies, especially endpoint mapping bodies.
- Smoke coverage-marker checks are secondary to actual `dotnet test` execution.

Residual maintainability risks:

- Some architecture tests intentionally use exact generated names and markers such as `MapEndpoints`, `AddProProfileServices`, `ServiceProviderCommandDispatcher`, and `HasDefaultSchema`. These are acceptable for a template where those generated APIs are part of the current contract, but intentional renames will require coordinated test updates.
- Smoke coverage assertions check specific test method names. That is useful for preventing silent coverage deletion, but it is still a marker check and should remain secondary to the generated tests themselves.
- Query non-mutation and endpoint no-persistence checks are source-convention checks. They catch generated-pattern regressions, not every possible semantic side effect.

No excessive runtime cost was observed beyond the expected template smoke matrix. The smoke suite remains substantial because it packs, generates, restores, builds, tests, and runs generated guardrails, but this is appropriate for template validation and remains idempotent.

## Validation Limitations

The reviewed limitations are acceptable for P1C:

- Domain isolation is tested at source-file and namespace level because the generated Modules project intentionally contains endpoint and infrastructure code, so assembly-level references would include ASP.NET Core and EF Core.
- Scalar cross-module IDs such as `ProjectId` remain allowed. P1C blocks cross-module FK configuration and cross-module Domain/Infrastructure namespace coupling, not scalar identity values.
- Generated FK configuration is blocked by absence checks for FK markers and by cross-module Domain/Infrastructure namespace checks.
- HTTP endpoints are not executed. Endpoint discipline is validated through generated source conventions and solution tests, not runtime HTTP calls.

These are validation boundaries rather than new unresolved architecture decisions, so no `OpenQuestions.md` update was made.

## Remaining P1D/P2 Gaps

Remaining work is outside P1C:

- P1D pro/advanced feature-depth work.
- Runtime API endpoint exercise beyond generated build/test.
- Broader pro/advanced behavioral tests beyond scaffolding/wiring.
- P2 public polish, screenshots, badges, docs site, release presentation, and public-facing polish.

Event sourcing remains out of scope and was not introduced.

## OpenQuestions.md Updates

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

## Spec Folder

Used spec folder:

```text
specs/0001-aegis-template-core
```

No spec files were updated.

## Closure

P1C is confirmed closed.

Recommended next goal: start P1D pro/advanced feature-depth work as a separate goal.
