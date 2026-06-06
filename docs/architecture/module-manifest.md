# Module Manifest

Each generated business module should include a lightweight `module.json` manifest.

The manifest is not a runtime framework requirement. It is metadata for humans, AI agents, documentation, and guardrails.

## Goals

- Make module ownership explicit.
- Document the database schema owned by the module.
- List public contracts and dependencies.
- Help agents avoid illegal cross-module access.
- Help guardrails validate boundaries.
- Enable future docs or diagram generation.

## Example

```json
{
  "name": "Tasks",
  "schema": "tasks",
  "type": "business-module",
  "owner": "core",
  "dependencies": [
    "Projects.Contracts"
  ],
  "publicContracts": [
    "TaskAssignedIntegrationEvent",
    "TaskCompletedIntegrationEvent"
  ],
  "features": [
    "CreateTask",
    "AssignTask",
    "CompleteTask",
    "ListProjectTasks"
  ],
  "rules": {
    "allowCrossModuleDatabaseAccess": false,
    "allowInfrastructureReferences": false
  }
}
```

## Rules

- Every module owns exactly one PostgreSQL schema by default.
- A module must not reference another module's Infrastructure.
- A module must not create cross-module foreign keys.
- A module may expose contracts through a Contracts namespace/project.
- Cross-module workflows should use contracts or integration events.

## Guardrails

The Node guardrail runner validates the manifest template and future implementations should validate generated manifests.
