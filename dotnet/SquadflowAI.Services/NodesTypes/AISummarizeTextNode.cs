using Microsoft.Extensions.Configuration;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class AISummarizeTextNode : INode
    {
        private readonly IOpenAIAPIClient _openAIAPIClient;
        private IConfiguration _configuration;
        private readonly IIntegrationsService _integrationsService;
        public AISummarizeTextNode(IOpenAIAPIClient openAIAPIClient, IConfiguration configuration,
            IIntegrationsService integrationsService) 
        {
            _openAIAPIClient = openAIAPIClient;
            _configuration = configuration;
            _integrationsService = integrationsService;
        }
        public string Id { get; private set; }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public async Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {
            //var prompt = parameters["prompt"];

            // Call your LLM API here 
            var offline = _configuration.GetValue<bool>("OFFLINE");
            if (!offline)
            {
                var integration = await _integrationsService.GetIntegrationByUserIdAsync((Guid)uIFlow.UserId);

                // LLM Call

                var configsForLLM = new RequestLLMDto();

                var systemPrompt = GenerateExtractContentSystemPrompt();
                configsForLLM.SystemPrompt = systemPrompt;
                configsForLLM.MaxTokens = 2000;

                configsForLLM.UserPrompt = GenerateExtractContentUserPrompt(input);

                var llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM, integration.OpenAIKey);

                return llmResponse?.Output;

            } else
            {
                return $"[Summary of '{input}'] using instruction: test";
            }

            //return $"[Summary of '{input}'] using instruction: {prompt}";
        }

        private string GenerateExtractContentSystemPrompt()
        {
            return $@"You are an advanced AI Text Summarizer. Your job is to generate concise, coherent summaries of provided input text while preserving the original meaning and key points. Use clear, professional language. Avoid adding opinions or unrelated details.

            If the input is lengthy or includes multiple sections, produce a structured summary that reflects the main ideas of each section.

            Your summaries should:
            - Be factually accurate and neutral
            - Be significantly shorter than the original input
            - Capture essential themes, points, or events
            - Avoid including metadata, boilerplate, or legal disclaimers
            ";
        }

        private string GenerateExtractContentUserPrompt(string textOrHtmls)
        {
            return $@"Summarize the following text or content extracted from an HTML page: {textOrHtmls}.";
        }
    }
}
