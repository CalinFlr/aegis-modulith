# Workflow: Review Spec

## Purpose

Review a spec before implementation or before merging a spec-driven change.

## Steps

1. Read `AGENTS.md`.
2. Read the target spec folder.
3. Check that goals and non-goals are clear.
4. Check that requirements are testable.
5. Check that acceptance criteria are observable.
6. Check that risks include technical, security, licensing, and adoption concerns.
7. Check that open questions are either local or promoted to root `OpenQuestions.md`.
8. Check that durable architecture decisions have ADRs.
9. Check that the plan can be implemented in small commits.

## Required validation

```bash
npm run check:specs
npm run check:docs
```

## Human approval

Required if the review identifies high-risk assumptions, missing approval gates, unclear licensing, or security-sensitive behavior.
