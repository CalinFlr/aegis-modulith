# Aegis.Modulith.Templates

Installs `dotnet new` templates for Aegis.Modulith:

- `aegis-modulith`
- `aegis-module`
- `aegis-slice`
- `aegis-event`
- `aegis-worker`

The main template generates an API-only modular monolith with CQRS-lite, PostgreSQL, vertical slices, optional MediatR, module manifests, profile scaffolding, and architecture tests.

Item templates are intended to be generated inside a generated solution. For module, slice, and event items, target `src/<App>.Modules/Modules/<Module>` and pass the generated namespaces:

```bash
dotnet new aegis-module -n Billing --schema billing --rootNamespace Acme.WorkHub.Modules --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks --buildingBlocksProject ../../../Acme.WorkHub.BuildingBlocks/Acme.WorkHub.BuildingBlocks.csproj -o src/Acme.WorkHub.Modules/Modules/Billing
dotnet new aegis-slice -n CreateInvoice --module Billing --kind command --mediator core --rootNamespace Acme.WorkHub.Modules --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks -o src/Acme.WorkHub.Modules/Modules/Billing
dotnet new aegis-event -n InvoiceIssued --module Billing --scope integration --rootNamespace Acme.WorkHub.Modules --buildingBlocksNamespace Acme.WorkHub.BuildingBlocks -o src/Acme.WorkHub.Modules/Modules/Billing
```
