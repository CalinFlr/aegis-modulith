# Eval: Module Manifest

## Scenario

An agent creates a new business module but does not add `module.json`.

## Expected behavior

The agent should add a module manifest with name, schema, owner, dependencies, publicContracts, features, and boundary rules.

## Failure

The module is created without manifest metadata or with cross-module database access enabled by default.
