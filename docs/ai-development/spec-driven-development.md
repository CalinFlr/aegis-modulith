# Spec-Driven Development

Aegis.Modulith uses specs to help humans and AI agents work on meaningful changes without relying on vague prompts.

## Why specs exist

AI agents are strongest when the work is framed as a durable contract:

```text
spec -> plan -> tasks -> acceptance -> implementation -> validation
```

Specs reduce hidden assumptions, improve traceability, and make the final result easier to review.

## Folder structure

```text
specs/
  README.md
  _template/
    spec.md
    plan.md
    tasks.md
    acceptance.md
    risks.md
    open-questions.md
  0001-aegis-template-core/
    spec.md
    plan.md
    tasks.md
    acceptance.md
    risks.md
    open-questions.md
```

## Relationship to OpenQuestions.md

Use spec-local `open-questions.md` for local unknowns.

Promote to root `OpenQuestions.md` when the question is:

- a blocker,
- a project-wide inferred decision,
- a public naming or template UX decision,
- a licensing/security/dependency issue,
- a durable architecture choice.

## Relationship to ADRs

Specs are for planned work. ADRs are for durable architectural decisions.

When a spec introduces or changes a major architectural rule, create or update an ADR.

## Agent workflow

Agents must:

1. Read `AGENTS.md`.
2. Read root `OpenQuestions.md`.
3. Read the relevant spec folder.
4. Follow the matching `.ai/workflows` file.
5. Implement tasks in small validated steps.
6. Update spec tasks and acceptance status.
7. Promote blockers or inferred cross-cutting decisions to root `OpenQuestions.md`.
8. Run relevant checks.
