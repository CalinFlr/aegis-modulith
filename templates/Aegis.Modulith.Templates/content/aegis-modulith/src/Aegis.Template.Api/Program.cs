using Aegis.Template.BuildingBlocks.Cqrs;
using Aegis.Template.Modules;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

#if (profile != "core")
using Aegis.Template.Api.Pro;
using Aegis.Template.ServiceDefaults;
#endif

#if (profile == "advanced")
using Aegis.Template.Api.Advanced;
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation())
    .WithTracing(tracing => tracing.AddAspNetCoreInstrumentation());
builder.Services.AddAegisDispatching(typeof(ModulesAssemblyMarker).Assembly);
builder.Services.AddAegisModules(builder.Configuration);

#if (profile != "core")
builder.AddServiceDefaults();
builder.Services.AddProProfileServices(builder.Configuration);
#endif

#if (profile == "advanced")
builder.Services.AddAdvancedProfileServices();
#endif

var app = builder.Build();

app.UseExceptionHandler();
#if (profile != "core")
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
#endif

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

#if (profile != "core")
app.MapProProfileEndpoints();
#endif

#if (profile == "advanced")
app.MapAdvancedProfileEndpoints();
#endif

app.Run();

public partial class Program;
