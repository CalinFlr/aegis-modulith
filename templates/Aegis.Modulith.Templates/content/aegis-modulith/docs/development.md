# Development

This generated app is API-only and uses a modular monolith structure.

## Daily checks

Run the .NET checks before committing:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

#if (guardrails != "off")
This repository also includes Node-based guardrails:

```bash
npm run check
```

#endif
## Module work

Keep features inside their owning module. Commands express business intent, queries do not mutate state, and API responses should use DTOs rather than EF entities.

Generate item templates under the modules project so namespaces match the generated architecture:

```bash
dotnet new aegis-module -n Billing --schema billing --rootNamespace Aegis.Template.Modules --buildingBlocksNamespace Aegis.Template.BuildingBlocks --buildingBlocksProject ../../../Aegis.Template.BuildingBlocks/Aegis.Template.BuildingBlocks.csproj -o src/Aegis.Template.Modules/Modules/Billing
dotnet new aegis-slice -n CreateInvoice --module Billing --kind command --mediator AegisMediatorValue --rootNamespace Aegis.Template.Modules --buildingBlocksNamespace Aegis.Template.BuildingBlocks -o src/Aegis.Template.Modules/Modules/Billing
dotnet new aegis-slice -n ListInvoices --module Billing --kind query --paged true --mediator AegisMediatorValue --rootNamespace Aegis.Template.Modules --buildingBlocksNamespace Aegis.Template.BuildingBlocks -o src/Aegis.Template.Modules/Modules/Billing
dotnet new aegis-event -n InvoiceIssued --module Billing --scope integration --rootNamespace Aegis.Template.Modules --buildingBlocksNamespace Aegis.Template.BuildingBlocks -o src/Aegis.Template.Modules/Modules/Billing
```

Run `dotnet build -c Release` after adding item templates. If you build the generated module item project directly, the aggregate modules project excludes nested `bin` and `obj` folders.
