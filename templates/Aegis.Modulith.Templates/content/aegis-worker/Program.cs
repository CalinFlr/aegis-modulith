using Aegis.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddAegis.Worker();

var app = builder.Build();
app.Run();
