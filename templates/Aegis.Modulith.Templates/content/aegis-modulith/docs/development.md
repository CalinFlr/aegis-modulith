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
