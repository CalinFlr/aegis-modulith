# Specs

This folder contains spec-driven development artifacts for Aegis.Modulith.

A spec is not a general TODO list. It is the contract used by humans and AI agents before implementing meaningful product work.

## Flow

1. Create a numbered spec folder, for example `specs/0002-add-outbox-dispatcher`.
2. Fill in `spec.md` with the problem, goals, non-goals, requirements, and acceptance summary.
3. Create `plan.md` with the implementation approach.
4. Create `tasks.md` with atomic tasks that can become commits or PR steps.
5. Create `acceptance.md` with validation commands and observable outcomes.
6. Create `risks.md` with technical, security, licensing, and product risks.
7. Create `open-questions.md` for spec-local unknowns, and promote cross-cutting questions to root `OpenQuestions.md`.

## Rules for agents

- Read `AGENTS.md` first.
- Read `OpenQuestions.md` before asking a human.
- Do not implement high-risk spec items without an approved plan.
- If a safe default exists, continue and record it as `inferred` in `OpenQuestions.md`.
- Promote durable architecture decisions from specs into ADRs.

## Required files per spec

- `spec.md`
- `plan.md`
- `tasks.md`
- `acceptance.md`
- `risks.md`
- `open-questions.md`
