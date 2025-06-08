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
            var promptOrFocuses = parameters["promptOrFocuses"];

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

                configsForLLM.UserPrompt = GenerateExtractContentUserPrompt(promptOrFocuses, input);

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
            return $@"You are an advanced AI Text Summarizer. Your job is to generate concise, coherent summaries of the provided input text, focusing specifically on the topics specified by the user.

                Use clear, professional language. Avoid adding opinions or unrelated details.

                If multiple topics are provided, produce a structured summary that addresses each topic separately. You may use bullet points or subheadings if helpful.

                Your summaries should:
                - Be factually accurate and neutral
                - Be significantly shorter than the original input
                - Focus only on the specified topics
                - Avoid including metadata, boilerplate, or legal disclaimers
                ";
        }


        private string GenerateExtractContentUserPrompt(string topics, string textOrHtmls)
        {
            return $@"Summarize the following content with a focus on these topics: {topics}

                Content:
                {textOrHtmls}";
        }

    }
}
