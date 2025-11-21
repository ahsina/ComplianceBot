using ComplianceBot.Application;
using ComplianceBot.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSerilog();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add Hangfire for background jobs
// services.AddHangfire(...)
// services.AddHangfireServer();

var host = builder.Build();
host.Run();
