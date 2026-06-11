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
- Skills: `AegisSkillsValue`
- Docs: `AegisDocsValue`
- License: `AegisLicenseExpressionValue`

## Validate

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
npm run check
```

#if (guardrails != "off")
AI and repository guardrails are available through:

```bash
npm run check
```

#endif
## Run

```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=aegis_template;Username=<user>;Password=<password>" --project src/Aegis.Template.Api
dotnet run --project src/Aegis.Template.Api
```
