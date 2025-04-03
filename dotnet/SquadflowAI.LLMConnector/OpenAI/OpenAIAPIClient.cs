using Newtonsoft.Json;
using SquadflowAI.Contracts;
using SquadflowAI.LLMConnector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SquadflowAI.LLMConnector.OpenAI
{
    public class OpenAIAPIClient : IOpenAIAPIClient
    {
        private readonly HttpClient _httpClient;
        public OpenAIAPIClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseLLMDto> SendMessageAsync(RequestLLMDto request)
        {
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            },
                max_tokens = request.MaxTokens > 0 ? request.MaxTokens : 500,
                temperature = 0.7
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await GetValidatedResponseAsync(content, request);

            //var parsedResponse = response?.choices?[0]?.message?.content;
            //var contentObject = JsonConvert.DeserializeObject<string>(parsedResponse);

            var mappedResult = JsonConvert.DeserializeObject<ResponseLLMDto>(response);

            return mappedResult;
        }

        private async Task<dynamic> GetValidatedResponseAsync(StringContent content, RequestLLMDto configsForLLM)
        {
            string response;
            int iteration = 0;
            do
            {
                iteration++;

                var result = await _httpClient.PostAsync("v1/chat/completions", content);
                var llmResponse = await result.Content.ReadAsStringAsync();

                if (IsResponseValid(llmResponse, out response)) break;

                configsForLLM.UserPrompt = $@"
                        The previous response did not match the required format. Please strictly adhere to the following format:
                        {{
                          ""input"": ""..."" or ""output"": ""..."",
                          ""completed"": false
                        }}

                        Retry with a valid response.";

            } while (true && iteration < 3);

            return response;
        }

        private bool IsResponseValid(string response, out string parsedResponse)
        {
            try
            {
                // Parse the top-level response
                var topLevelResponse = JsonConvert.DeserializeObject<dynamic>(response);

                // Extract the 'content' field from the structure
                parsedResponse = topLevelResponse?.choices?[0]?.message?.content;

                if (parsedResponse == null)
                {
                    return false; // Content is missing
                }

                string cleanedResponse = Regex.Replace(parsedResponse, @"```json|```", "").Trim();
                var contentObject = JsonConvert.DeserializeObject<ResponseLLMDto>(cleanedResponse);

                // Check if 'completed' exists and at least one of 'input' or 'output' exists
                return contentObject != null;
            }
            catch
            {
                // On failure, set parsedResponse to null and return false
                parsedResponse = null;
                return false;
            }
        }

    }
}
