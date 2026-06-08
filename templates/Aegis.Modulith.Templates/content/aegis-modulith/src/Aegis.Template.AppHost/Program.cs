#if (profile != "core")
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.Build().Run();
#endif
