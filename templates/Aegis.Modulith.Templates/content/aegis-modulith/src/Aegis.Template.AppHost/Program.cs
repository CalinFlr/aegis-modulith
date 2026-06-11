#if AEGIS_PRO_OR_ADVANCED
var builder = Aspire.Hosting.DistributedApplication.CreateBuilder(args);

builder.Build().Run();
#else
Console.WriteLine("Aegis.Template AppHost is enabled by the pro and advanced profiles.");
#endif
