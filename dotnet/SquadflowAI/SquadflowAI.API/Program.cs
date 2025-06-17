using Microsoft.OpenApi.Models;
using SquadflowAI.Infrastructure;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.LLMConnector.OpenAI;
using SquadflowAI.Services.Agent;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.LLMExecutors;
using SquadflowAI.Services.NodesTypes;
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
//builder.Services.AddOpenApi();

// Get configuration
var configuration = builder.Configuration;
var openAIApiKey = configuration.GetValue<string>("OPENAI_API_KEY");
var serperApiKey = configuration.GetValue<string>("SERPER_API_KEY");

var gmailEmail = configuration.GetValue<string>("GMAIL_EMAIL");
var gmailAppPassword = configuration.GetValue<string>("GMAIL_APPPASWORD");

builder.Services.AddHttpClient<IOpenAIAPIClient, OpenAIAPIClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    // client.DefaultRequestHeaders.Add("Authorization", $"Bearer {openAIApiKey}");
});

builder.Services.AddHttpClient<ISerperAPIClient, SerperAPIClient>(client =>
{
    client.BaseAddress = new Uri("https://google.serper.dev/search");
   // client.DefaultRequestHeaders.Add("X-API-KEY", $"{serperApiKey}");
});

builder.Services.AddHttpClient<IWebScraper, WebScraper>(client =>
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
builder.Services.AddTransient<IUIFlowService, UIFlowService>();
builder.Services.AddTransient<IAgentService, AgentService>();
builder.Services.AddTransient<IOpenAILLMExecutorServiceOLD, OpenAILLMExecutorServiceOLD>();
builder.Services.AddTransient<IToolsService, ToolsService>();
builder.Services.AddTransient<IProjectsService, ProjectsService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IFlowExecutorService, FlowExecutorService>();
builder.Services.AddTransient<IIntegrationsService, IntegrationsService>();


builder.Services.AddTransient<TextInputNode>();
builder.Services.AddTransient<LLMPromptNode>();
builder.Services.AddTransient<AISummarizeTextNode>();
builder.Services.AddTransient<TextOutputNode>();
builder.Services.AddTransient<WebResearchNode>();
builder.Services.AddTransient<PdfInputNode>();


builder.Services.AddSingleton<NodeFactory>();


// Repositories
builder.Services.AddScoped<IUIFlowRepository, UIFlowRepository>();
builder.Services.AddScoped<IAgentRepository, AgentRepository>();
builder.Services.AddScoped<IActionRunRepository, ActionRunRepository>(); 
builder.Services.AddScoped<IToolsRepository, ToolsRepository>();
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IIntegrationsRepository, IntegrationsRepository>();

//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SquadflowAI API",
        Version = "v1",
        Description = "API for SquadflowAI services",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@squadflowai.com",
        }
    });
});


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    initializer.EnsureDatabaseSetup();
}

app.UseCors(options =>
{
    // options.WithOrigins("http://localhost:3000");
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SquadflowAI.API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
