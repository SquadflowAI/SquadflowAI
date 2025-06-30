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

        public async Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]>? parametersByte = null)
        {
            var promptOrFocuses = parameters["promptOrFocuses"];

            // Call your LLM API here 
            var offline = _configuration.GetValue<bool>("OFFLINE");
            if (!offline)
            {
                var integration = await _integrationsService.GetIntegrationsByUserIdAsync((Guid)uIFlow.UserId, false);

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
            return $@"You are an advanced AI Text Summarizer. Your role is to create clear, concise, and well-organized summaries based on the user’s input content and focus topics.

            Your summaries must:
            - Be significantly shorter than the original content
            - Be factually accurate and neutral
            - Focus only on the user’s requested topics
            - Avoid including metadata, disclaimers, or unrelated boilerplate

            Adapt your summary to match the domain of the content. When appropriate, organize information into meaningful sections or use subheadings (e.g., for technical reports, news, scientific papers, business updates, etc.). 

            **Use spacing and line breaks generously** to separate titles, sections, and paragraphs, so the output is easy to read and never looks like one large block of text.

            You must be flexible:
            - Use professional, domain-appropriate language
            - Match the tone and depth to the subject (e.g., simple for general news, more technical for scientific topics)
            - Be adaptable to all content types: academic, corporate, government, financial, media, scientific, etc.

            If multiple topics are given, address each clearly and distinctly.";
        }




        private string GenerateExtractContentUserPrompt(string topics, string textOrHtmls)
        {
            return $@"Summarize the following content with a focus on these topics: {topics}

            Ensure the summary is organized, domain-appropriate, and easy to read. Use sections or logical grouping where it helps clarity.

            Content:
            {textOrHtmls}";
        }


    }
}
