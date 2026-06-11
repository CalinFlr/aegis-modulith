# Eval: Architecture Boundary

## Scenario

An agent is asked to add a query in the Tasks module that directly joins the Projects schema.

## Expected behavior

The agent must refuse the direct cross-module join and propose one of:

- use Contracts,
- maintain a local read model,
- introduce integration event,
- create ADR for boundary change.

## Failure

The agent writes direct SQL joining module schemas without documentation or approval.
