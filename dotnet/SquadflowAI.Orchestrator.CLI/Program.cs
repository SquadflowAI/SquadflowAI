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
using SquadflowAI.Services.Agent;
using Newtonsoft.Json;
using SquadflowAI.Tools.Serper;
using SquadflowAI.Tools.Interfaces;
using SquadflowAI.Tools.WebScraper;
using System.Net.Http;
using SquadflowAI.Services.LLMExecutors;
using SquadflowAI.Tools.DataAnalyzer;
using SquadflowAI.Tools.PdfGenerator;
using SquadflowAI.Tools.GmailClient;

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
        var serperApiKey = configuration.GetValue<string>("SERPER_API_KEY");

        var gmailEmail = configuration.GetValue<string>("GMAIL_EMAIL");
        var gmailAppPassword = configuration.GetValue<string>("GMAIL_APPPASWORD");

        services.AddHttpClient<IOpenAIAPIClient, OpenAIAPIClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAIApiKey}");
        });

        services.AddHttpClient<ITool, SerperAPIClient>(client =>
        {
            client.BaseAddress = new Uri("https://google.serper.dev/search");
            client.DefaultRequestHeaders.Add("X-API-KEY", $"{serperApiKey}");
        });

        services.AddHttpClient<ITool, WebScraper>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
            client.DefaultRequestHeaders.Add("Referer", "https://www.google.com/");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
        });

        services.AddTransient<ITool, DataAnalyzer>();
        services.AddTransient<ITool, PdfGenerator>();
        services.AddTransient<ITool, GmailClient>();
        

        // Register services
        services.AddSingleton<DbContext>();
        services.AddTransient<DatabaseInitializer>();
        services.AddTransient<IAgentService, AgentService>();
        services.AddTransient<IOpenAILLMExecutorService, OpenAILLMExecutorService>();
        // Repositories
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IActionRunRepository, ActionRunRepository>();

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

var agentService = app.Services.GetRequiredService<IAgentService>();

//await agentService.CreateAgentAsync();

var result = await agentService.GetAgentByNameAsync("Football Stats Agent");
Console.WriteLine(JsonConvert.SerializeObject(result));

var exec = app.Services.GetRequiredService<IOpenAILLMExecutorService>();

await exec.ExecuteAsync(result);
//--TEST AREA END

Console.WriteLine("Agent Name:");

Console.WriteLine("LLM Api key:");

Console.ReadKey();

