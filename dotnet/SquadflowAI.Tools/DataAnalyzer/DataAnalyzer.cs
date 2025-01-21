
using SquadflowAI.Contracts;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.DataAnalyzer
    {
        public class DataAnalyzer : ITool
        {
            public string Key => "data-analyzer";

            private IOpenAIAPIClient _openAIAPIClient;

            public DataAnalyzer(IOpenAIAPIClient openAIAPIClient)
            {
                _openAIAPIClient = openAIAPIClient;
            }

            public async Task<string> ExecuteAsync(ToolConfigDto configs)
            {
                var configsForLLM = new RequestLLMDto();
                configs.Inputs.TryGetValue("Name", out dynamic name);
                configs.Inputs.TryGetValue("Description", out dynamic description);
                configs.Inputs.TryGetValue("ActionToExecute", out dynamic actionToExecute);
                configs.Inputs.TryGetValue("Inputs", out dynamic inputs);
                configs.Inputs.TryGetValue("Outputs", out dynamic outputs);
                configs.Inputs.TryGetValue("Data", out dynamic data);

                // Generate dynamic system and user prompts based on the configuration JSON
                configsForLLM.SystemPrompt = $@"
                    You are an intelligent assistant specialized in analyzing data based on user-provided configurations. 
                    Here are the details of the task configuration:

                    Name: {name}
                    Description:  {description}
                    Action to Execute: {actionToExecute}
                    Inputs:  {inputs}
                    Outputs: {outputs}

                    Your job is to interpret the raw data provided by the user and generate clear and actionable insights as specified in the configuration. Be precise, concise, and accurate in your analysis.
                    ";

                configsForLLM.UserPrompt = $@"
                    The user has provided the following raw data for analysis:

                   {data}

                    Based on the configuration, perform the following actions:
                    - {actionToExecute}

                    Your response should include:
                    - {string.Join("\n- ", outputs)}

                    
                Strictly adhere to the following response format:
                 {{
                  ""output"": ""your response. Stricly avoid json in your response. Only text"",
                  ""completed"": boolean true or false
                 }}

                

                Ensure that the completed field indicates whether the action is done. If one more iteration is needed then set to false.";

                // Send the prompts to the LLM and await the response
                var llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM);

                return llmResponse.Output;
            }
        }
    }