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
```

#if (guardrails != "off")
AI and repository guardrails are available through:

```bash
npm run check
```

#endif
## Run

```bash
dotnet run --project src/Aegis.Template.Api
```
