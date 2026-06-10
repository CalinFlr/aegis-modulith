FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY Directory.Packages.props ./
COPY src/Aegis.Template.BuildingBlocks/Aegis.Template.BuildingBlocks.csproj src/Aegis.Template.BuildingBlocks/
COPY src/Aegis.Template.Modules/Aegis.Template.Modules.csproj src/Aegis.Template.Modules/
COPY src/Aegis.Template.ServiceDefaults/Aegis.Template.ServiceDefaults.csproj src/Aegis.Template.ServiceDefaults/
COPY src/Aegis.Template.Api/Aegis.Template.Api.csproj src/Aegis.Template.Api/

RUN dotnet restore src/Aegis.Template.Api/Aegis.Template.Api.csproj

COPY . .
RUN dotnet publish src/Aegis.Template.Api/Aegis.Template.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Aegis.Template.Api.dll"]
