FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "Aegis.Template.Api.dll"]
