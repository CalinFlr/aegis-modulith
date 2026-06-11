# Open Questions

This file is the shared decision queue between AI agents and human maintainers.

It exists so agents can continue implementation with explicit assumptions instead of silently guessing or repeatedly stopping for non-blocking clarification.

## Rules

1. Do not use this file for ordinary TODOs. Use it only for questions, blockers, assumptions, and decisions that need human visibility.
2. Do not stop work for low-risk unknowns. Make a reasonable assumption, mark it as `inferred`, and continue.
3. Stop work only for true blockers: security, licensing, irreversible architecture choices, missing runtimes, missing package access, data loss risk, or anything that prevents validation.
4. Every entry must include a proposed default and the impact if the human chooses differently.
5. When a human answers, move the entry to `Answered or decided` and update the related docs, ADRs, code, or tests.
6. Do not delete resolved entries until the end of a milestone; preserve decision traceability.
7. Spec-local questions live in `specs/<id>/open-questions.md`; promote them here when they affect public architecture, security, licensing, CI/CD, template UX, dependencies, or release.

## Status values

- `open`: needs human answer but does not block current work.
- `blocker`: current milestone cannot be completed safely without a human answer.
- `inferred`: agent made a reasonable assumption and continued.
- `answered`: human answered, implementation not yet updated.
- `decided`: answer has been applied to code/docs/tests.
- `deferred`: intentionally postponed.

## Risk values

- `low`: naming, wording, minor docs, local convention.
- `medium`: public API shape, template UX, generated folder structure.
- `high`: security, licensing, dependencies, auth/authz, database migrations, public contracts.
- `critical`: release, package publishing, destructive commands, data loss, CI bypass, license change.

## Open or inferred questions

<!--
Use this template:

### Q-YYYYMMDD-001: Short question title

- Status: open | blocker | inferred | answered | decided | deferred
- Risk: low | medium | high | critical
- Owner: human | agent | maintainer
- Source: user request | implementation | validation | dependency | security review
- Affected areas: files, modules, templates, docs, CI, packages
- Question: What needs a decision?
- Context: Why did this come up?
- Proposed default: What should the agent do if no answer is available?
- Impact if different: What changes if the human chooses another path?
- Current action: continue | stop | implement default | document only
- Created: YYYY-MM-DD
- Resolved: YYYY-MM-DD or N/A
-->

### Q-20260606-001: Confirm final public project name before publish

- Status: inferred
- Risk: medium
- Owner: human
- Source: naming decision
- Affected areas: NuGet package, GitHub repository, namespaces, README, docs
- Question: Should the final public name remain `Aegis.Modulith`?
- Context: `Aegis.Modulith` was selected as the working name because it communicates protection/guardrails and keeps `Modulith` for discoverability.
- Proposed default: Continue implementation with `Aegis.Modulith`.
- Impact if different: Rename package IDs, namespaces, template names, docs, GitHub repo metadata, and sample names before public release.
- Current action: continue
- Created: 2026-06-06
- Resolved: N/A

### Q-20260606-002: Adopt spec-driven development layer

- Status: inferred
- Risk: medium
- Owner: human
- Source: competitive review and AI development design
- Affected areas: specs, workflows, skills, guardrails, docs, template strategy
- Question: Should Aegis.Modulith include a spec-driven development layer inspired by modern AI-assisted development workflows?
- Context: Competitive review showed that spec-driven workflows are useful for long-running agent work and reduce silent assumptions.
- Proposed default: Include `specs/` with templates and require specs for medium/high-risk work.
- Impact if different: Remove specs, related workflows, guardrails, and docs; rely only on AGENTS.md, OpenQuestions.md, and ADRs.
- Current action: continue
- Created: 2026-06-06
- Resolved: N/A

### Q-20260606-003: Include module.json manifests in generated modules

- Status: inferred
- Risk: medium
- Owner: human
- Source: module boundary and AI-agent context design
- Affected areas: aegis-module template, docs, guardrails, architecture tests, generated modules
- Question: Should each generated module include a lightweight `module.json` manifest?
- Context: Module metadata helps humans and AI agents understand ownership, schema, dependencies, public contracts, features, and boundary rules.
- Proposed default: Include `module.json` manifests as metadata only, not as a heavy runtime framework.
- Impact if different: Remove manifest generation and rely on docs/conventions/tests only.
- Current action: continue
- Created: 2026-06-06
- Resolved: N/A

### Q-20260606-004: Pin optional MediatR to Apache-2.0 package line

- Status: inferred
- Risk: high
- Owner: human
- Source: dependency and licensing review
- Affected areas: template package, generated Directory.Packages.props, mediator option docs, smoke tests
- Question: Should the optional `--mediator mediatr` path pin MediatR to version `12.5.0` instead of the current NuGet latest line?
- Context: NuGet resolved MediatR `14.1.0`, whose package metadata requires license acceptance and points to RPL/commercial terms. MediatR `12.5.0` restores on .NET 10 and declares `Apache-2.0` license metadata.
- Proposed default: Pin optional MediatR to `12.5.0` and keep the core dispatcher as the default.
- Impact if different: Moving to the newer MediatR line would require explicit maintainer license approval and updated generated documentation.
- Current action: implement default
- Created: 2026-06-06
- Resolved: N/A

### Q-20260608-001: Define the generated core AI skill subset

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1A generated AI option materialization
- Affected areas: templates, generated `.agents/skills`, generated AI docs, smoke assertions
- Question: Which skills should `--skills core` include in generated enterprise AI output?
- Context: The CLI documents `--skills none|core|enterprise`, but the previous implementation did not materialize a generated skill set. P1A needed a concrete, testable core subset.
- Proposed default: Treat `core` as the skills needed for basic docs, architecture, module, vertical-slice, module-manifest, and spec workflows: `docs-writer`, `dotnet-architecture-review`, `dotnet-module`, `dotnet-vertical-slice`, `module-manifest`, and `spec-driven-feature`.
- Impact if different: Update generated `.agents/skills`, generated AI docs, and smoke assertions to match the maintainer-selected core subset.
- Current action: implement default
- Created: 2026-06-08
- Resolved: N/A

## Blockers

No known blockers at pack creation time.

## Answered or decided

No answered questions yet.
