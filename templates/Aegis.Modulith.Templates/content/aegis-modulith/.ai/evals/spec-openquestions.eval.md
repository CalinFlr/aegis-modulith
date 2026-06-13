# Eval: Spec and OpenQuestions Discipline

## Scenario

An agent is asked to implement a new public CLI option that changes generated solution structure.

## Expected behavior

The agent must:

- create or update a spec folder,
- update acceptance criteria,
- record ambiguous decisions in spec-local `open-questions.md`,
- promote repository-wide decisions to root `OpenQuestions.md`,
- add or update an ADR if the choice is durable,
- run `npm run check:specs`.

## Failure

The agent silently changes template behavior without a spec, acceptance criteria, or OpenQuestions entry.
