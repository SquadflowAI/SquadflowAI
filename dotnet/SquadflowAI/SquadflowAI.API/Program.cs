using Microsoft.OpenApi.Models;
using SquadflowAI.Infrastructure;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.LLMConnector.OpenAI;
using SquadflowAI.Services.Agent;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.LLMExecutors;
using SquadflowAI.Services.Services;
using SquadflowAI.Tools.DataAnalyzer;
using SquadflowAI.Tools.GmailClient;
using SquadflowAI.Tools.Interfaces;
using SquadflowAI.Tools.PdfGenerator;
using SquadflowAI.Tools.Serper;
using SquadflowAI.Tools.WebScraper;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Get configuration
var configuration = builder.Configuration;
var openAIApiKey = configuration.GetValue<string>("OPENAI_API_KEY");
var serperApiKey = configuration.GetValue<string>("SERPER_API_KEY");

var gmailEmail = configuration.GetValue<string>("GMAIL_EMAIL");
var gmailAppPassword = configuration.GetValue<string>("GMAIL_APPPASWORD");

builder.Services.AddHttpClient<IOpenAIAPIClient, OpenAIAPIClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAIApiKey}");
});

builder.Services.AddHttpClient<ITool, SerperAPIClient>(client =>
{
    client.BaseAddress = new Uri("https://google.serper.dev/search");
    client.DefaultRequestHeaders.Add("X-API-KEY", $"{serperApiKey}");
});

builder.Services.AddHttpClient<ITool, WebScraper>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
    client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
    client.DefaultRequestHeaders.Add("Referer", "https://www.google.com/");
    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
    client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
});

builder.Services.AddTransient<ITool, DataAnalyzer>();
builder.Services.AddTransient<ITool, PdfGenerator>();
builder.Services.AddTransient<ITool, GmailClient>();

builder.Services.AddSingleton<DbContext>();
builder.Services.AddTransient<DatabaseInitializer>();
builder.Services.AddTransient<IUIOrchestrationService, UIOrchestrationService>();
builder.Services.AddTransient<IAgentService, AgentService>();
builder.Services.AddTransient<IOpenAILLMExecutorService, OpenAILLMExecutorService>();

// Repositories
builder.Services.AddScoped<IUIOrchestrationRepository, UIOrchestrationRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IActionRunRepository, ActionRunRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SquadflowAI.API", Version = "v1" });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.EnsureDatabaseSetup();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    // options.WithOrigins("http://localhost:3000");
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquadflowAI.API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

app.UseAuthorization();

app.MapControllers();

app.Run();
