# Getting Started

## Restore and build

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Run the API

```bash
dotnet run --project src/Aegis.Template.Api
```

## Generated options

- Profile: `AegisProfileValue`
- Mediator: `AegisMediatorValue`
- AI assets: `AegisAiValue`
- Guardrails: `AegisGuardrailsValue`
- Docs: `AegisDocsValue`
- License: `AegisLicenseExpressionValue`
