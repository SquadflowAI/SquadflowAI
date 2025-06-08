using Microsoft.Extensions.Configuration;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.NodesTypes.Base;
using SquadflowAI.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class WebResearchNode : INode
    {
        private readonly ISerperAPIClient _serperAPIClient;
        private readonly IIntegrationsService _integrationsService;
        private readonly IWebScraper _webScraper;
        private readonly IOpenAIAPIClient _openAIAPIClient;
        private IConfiguration _configuration;
        private readonly AISummarizeTextNode _aISummarizeTextNode;

        public WebResearchNode(ISerperAPIClient serperAPIClient, IIntegrationsService integrationsService, IConfiguration configuration,
            IWebScraper webScraper, IOpenAIAPIClient openAIAPIClient, AISummarizeTextNode aISummarizeTextNode)
        {
            _serperAPIClient = serperAPIClient;
            _integrationsService = integrationsService;
            _configuration = configuration;
            _webScraper = webScraper;
            _openAIAPIClient = openAIAPIClient;
            _aISummarizeTextNode = aISummarizeTextNode;
        }


        public string Id => throw new NotImplementedException();

        public async Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {
            var query = parameters["prompt"] + " " + input;

            var offline = _configuration.GetValue<bool>("OFFLINE");
            if (!offline)
            {
                var integration = await _integrationsService.GetIntegrationByUserIdAsync((Guid)uIFlow.UserId);
                var serperAPIResponse = await _serperAPIClient.ExecuteAsync(query, integration.SerperAPIKey);

                // Foreach page extract html
                var links = serperAPIResponse.Organic.Select(x => x.Link).Take(10);                
                var content = await _webScraper.ExecuteAsync(links);

                // LLM Extracts the Content

                var configsForLLM = new RequestLLMDto();

                //var systemPrompt = GenerateExtractContentSystemPrompt();
                //configsForLLM.SystemPrompt = systemPrompt;

                //configsForLLM.MaxTokens = 2000;

                //configsForLLM.UserPrompt = GenerateExtractContentUserPrompt(content);

                //var llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM, integration.OpenAIKey);

                // Call Summarizer Text

                var inputDict = new Dictionary<string, string>();
                inputDict.Add("promptOrFocuses", input);

                var textSummarizationResult = await _aISummarizeTextNode.ExecuteAsync(content, inputDict, uIFlow);

                return textSummarizationResult;

            }
            else
            {
                return "WebReasearch proccesed something";
            }
            

        }

        private string GenerateExtractContentSystemPrompt()
        {
            return $@"You are an intelligent and precise HTML content extractor. Your task is to extract meaningful human-readable text content from provided HTML input(s). Ignore all HTML tags, scripts, styles, metadata, and structural elements. Do not hallucinate or summarize. Preserve the original order of content as it appears in the HTML.

            If multiple HTML documents are provided, extract the text from each one separately and label them clearly (e.g., ""Document 1"", ""Document 2"", etc.).

            Always return clean, plain text suitable for NLP or further analysis.
            ";
        }

        private string GenerateExtractContentUserPrompt(string htmls)
        {
            return $@" Extract the text content from the following HTML/HTMLs: {htmls}.";
        }
    }
}
