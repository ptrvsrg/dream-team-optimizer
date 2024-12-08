using DreamTeamOptimizer.Core.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DreamTeamOptimizer.MsEmployee;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using (var scope = host.Services.CreateScope())
        {
            using (var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                ctx.Database.Migrate();
            }
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder.AddEnvironmentVariables())
            .ConfigureLogging((context, builder) =>
            {
                var config = new LoggerConfiguration()
                    .ReadFrom.Configuration(context.Configuration.GetSection("Logging"));
                Log.Logger =  config.CreateLogger();
                builder.ClearProviders().AddSerilog();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://*:8080");
            })
            .UseSerilog();
    }
}