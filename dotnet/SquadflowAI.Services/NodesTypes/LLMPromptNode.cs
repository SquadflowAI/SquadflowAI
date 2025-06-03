using Microsoft.Extensions.Configuration;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.LLMConnector.OpenAI;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class LLMPromptNode : INode
    {
        private readonly IOpenAIAPIClient _openAIAPIClient;
        private readonly IIntegrationsService _integrationsService;
        private IConfiguration _configuration;
        public string Id { get; private set; }

        public LLMPromptNode(IOpenAIAPIClient openAIAPIClient, IIntegrationsService integrationsService, IConfiguration configuration)
        {
            _openAIAPIClient = openAIAPIClient;
            _integrationsService = integrationsService;
            _configuration = configuration;
        }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public async Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {
            //var prompt = parameters["prompt"].Replace("{{input}}", input);
            var prompt = parameters["prompt"] + " " + input;

            //LLM API here
            var offline = _configuration.GetValue<bool>("OFFLINE");
            if (!offline)
            {
                var integration = await _integrationsService.GetIntegrationByUserIdAsync((Guid)uIFlow.UserId);
                var configsForLLM = new RequestLLMDto();

                var systemPrompt = GenerateSystemPrompt();
                configsForLLM.SystemPrompt = systemPrompt;

                configsForLLM.MaxTokens = 2000;

                configsForLLM.UserPrompt = prompt;

                var llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM, integration.OpenAIKey);

                return llmResponse?.Output;
            } else
            {
                return "LLM proccesed something";
            }
        }

        private string GenerateSystemPrompt()
        {
            //var capabilitiesList = string.Join(", ", capabilities.Select((cap, index) => $"{index + 1}. {cap.Task}: {cap.Description}"));

            return $@"You are an intelligent assistant. Follow instructions carefully.";
        }
    }
}
