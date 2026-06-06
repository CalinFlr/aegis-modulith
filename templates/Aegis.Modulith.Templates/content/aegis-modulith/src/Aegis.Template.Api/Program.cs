using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules;

#if AEGIS_PRO_OR_ADVANCED
using Aegis.Template.Api.Pro;
using Aegis.Template.ServiceDefaults;
#endif

#if AEGIS_ADVANCED
using Aegis.Template.Api.Advanced;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddAegisDispatching(typeof(ModulesAssemblyMarker).Assembly);
builder.Services.AddAegisModules(builder.Configuration);

#if AEGIS_PRO_OR_ADVANCED
builder.AddServiceDefaults();
builder.Services.AddProProfileServices();
#endif

#if AEGIS_ADVANCED
builder.Services.AddAdvancedProfileServices();
#endif

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new
{
    application = "Aegis.Template",
    profile = "AegisProfileValue",
    mediator = "AegisMediatorValue",
    sample = "AegisSampleValue"
})).WithName("GetApplicationInfo");

app.MapAegisModules();

#if AEGIS_PRO_OR_ADVANCED
app.MapProProfileEndpoints();
#endif

#if AEGIS_ADVANCED
app.MapAdvancedProfileEndpoints();
#endif

app.Run();
