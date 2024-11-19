using DreamTeamOptimizer.ConsoleApp.DI;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Services;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

HostApplicationBuilderSettings settings = new()
{
    Args = args,
    Configuration = new ConfigurationManager(),
    ContentRootPath = Directory.GetCurrentDirectory(),
};
settings.Configuration.AddEnvironmentVariables(prefix: "HACKATHON_");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(settings);
builder.Logging.ClearProviders().AddSerilog();
builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureStrategy(builder.Configuration);
builder.Services.AddHostedService<HackathonWorker>();

using var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

host.Run();