using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SquadflowAI.Infrastructure;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.LLMConnector.OpenAI;
using System;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.AgentBuilder;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        // Add configuration from appsettings.json
        config.SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Get configuration
        var configuration = context.Configuration;
        var openAIApiKey = configuration.GetValue<string>("OPENAI_API_KEY");

        services.AddHttpClient<IOpenAPIClient, OpenAPIClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAIApiKey}");
        });

        // Register services
        services.AddSingleton<DbContext>();
        services.AddTransient<DatabaseInitializer>();
        services.AddTransient<IAgentService, AgentService>();
        // Repositories
        services.AddScoped<IAgentRepository, AgentRepository>();

    });

var app = builder.Build();

// Run database initialization
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.EnsureDatabaseSetup();
}

Console.WriteLine("Database initialized. Application is running!");

//--TEST AREA

//var openAIService = app.Services.GetRequiredService<IOpenAPIClient>();

//string prompt = "Explain quantum computing in simple terms.";
//var response = await openAIService.SendMessageAsync(prompt);

//Console.WriteLine(response);

var agentService = app.Services.GetRequiredService<IAgentService>();

string prompt = "Explain quantum computing in simple terms.";
await agentService.CreateAgentAsync();

//--TEST AREA END

Console.WriteLine("Agent Name:");

Console.WriteLine("LLM Api key:");

Console.ReadKey();

