# Open Questions Policy

`OpenQuestions.md` is the human-agent decision queue for Aegis.Modulith.

The purpose is not to create bureaucracy. The purpose is to make AI-assisted development safer and more transparent by separating:

- true blockers that require a human answer;
- inferred decisions where the agent can continue with a documented assumption;
- deferred questions that should not slow the current milestone;
- answered decisions that still need to be applied to code, docs, tests, or ADRs.

## Why this exists

AI agents are good at continuing work, but they can hide assumptions. Hidden assumptions are dangerous in enterprise development, especially around licensing, security, public APIs, dependencies, migrations, auth/authz, and release automation.

`OpenQuestions.md` gives the agent a disciplined place to record uncertainty without turning every unknown into an interruption.

## When to add an entry

Add an entry when one of these is true:

- the agent cannot validate the current milestone;
- a required SDK, runtime, package, or tool is unavailable;
- a decision affects public package names, repository names, or template short names;
- a dependency or license decision is unclear;
- a security, auth, authorization, secrets, or CI policy decision is unclear;
- a database migration, data deletion, or irreversible operation is involved;
- a user preference is unknown, but the agent can continue safely with a documented default;
- the agent infers a decision that should be visible to a maintainer.

Do not add entries for normal implementation tasks. Those belong in issues, TODOs, roadmap items, or the current milestone plan.

## Blocker versus inferred decision

Use `blocker` only when continuing would be unsafe, misleading, or impossible.

Examples of blockers:

- cannot run `dotnet` and template validation is required;
- package cannot be restored;
- license conflict prevents adding a dependency;
- human approval is required before modifying auth/authz;
- a destructive command or release/publish operation is needed.

Use `inferred` when the agent can continue with a reasonable default.

Examples of inferred decisions:

- exact wording of README sections;
- naming of internal folders;
- initial sample data choices;
- choosing `Aegis.Modulith` as working name before public publish;
- using `pro` as the default profile because the project goal says common API infrastructure should be generated.

## Required fields

Every entry must include:

- status;
- risk;
- owner;
- source;
- affected areas;
- question;
- context;
- proposed default;
- impact if different;
- current action;
- created date;
- resolved date or `N/A`.

## Agent behavior

Agents must:

1. read `OpenQuestions.md` before substantial implementation;
2. add blockers as soon as they are discovered;
3. add inferred decisions when making meaningful assumptions;
4. continue when the question is low or medium risk and a safe default exists;
5. stop when the question is high or critical risk and no safe default exists;
6. summarize open blockers and inferred decisions in the final report.

## Human behavior

Humans should answer by editing the entry and changing status to `answered` or `decided`.

When an answer changes architecture or policy, update the related ADR, docs, tests, templates, and guardrails in the same milestone.

## Relationship to ADRs

`OpenQuestions.md` is not a replacement for ADRs.

Use `OpenQuestions.md` for active uncertainty.
Use ADRs for accepted architectural decisions.

When a question becomes a durable architectural decision, create or update an ADR and mark the question as `decided`.
