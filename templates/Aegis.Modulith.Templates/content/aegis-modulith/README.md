# Aegis.Template

Generated with `Aegis.Modulith`.

## Shape

- Profile: `AegisProfileValue`
- Mediator: `AegisMediatorValue`
- Database: `AegisDatabaseValue`
- Sample: `AegisSampleValue`
- AI assets: `AegisAiValue`
- Guardrails: `AegisGuardrailsValue`
- Hooks: `AegisHooksValue`
- Skill generation: `AegisSkillsValue` applies only when enterprise AI assets are generated.
- Docs: `AegisDocsValue`
- License: `AegisLicenseExpressionValue`

## Validate

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Run

```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=aegis_template;Username=<user>;Password=<password>" --project src/Aegis.Template.Api
dotnet run --project src/Aegis.Template.Api
```
