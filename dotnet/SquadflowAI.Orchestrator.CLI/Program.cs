using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SquadflowAI.Infrastructure;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Infrastructure.Interfaces;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add configuration from appsettings.json
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Get configuration
        var configuration = context.Configuration;

        // Register services
        services.AddSingleton<DbContext>();
        services.AddTransient<DatabaseInitializer>();
        // Repositories
        services.AddScoped<IAgentConfigurationRepository, AgentConfigurationRepository>();

    });

var app = builder.Build();

// Run database initialization
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.EnsureDatabaseSetup();
}

Console.WriteLine("Database initialized. Application is running!");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();


Console.WriteLine("Agent Name:");

Console.WriteLine("LLM Api key:");

