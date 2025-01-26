using Newtonsoft.Json;
using SquadflowAI.Contracts;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.LLMConnector.OpenAI;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Tools;
using SquadflowAI.Tools.Interfaces;

namespace SquadflowAI.Services.LLMExecutors
{
    public class OpenAILLMExecutorService : IOpenAILLMExecutorService
    {
        private readonly ExecutionContext _context;
        private IOpenAIAPIClient _openAIAPIClient;
        private IActionRunRepository _actionRunRepository;
        private readonly Dictionary<string, ITool> _tools;
        public OpenAILLMExecutorService(IOpenAIAPIClient openAIAPIClient, IEnumerable<ITool> tools,
            IActionRunRepository actionRunRepository) 
        {
            _openAIAPIClient = openAIAPIClient;
            _context = new ExecutionContext();
            _tools = tools.ToDictionary(strategy => strategy.Key);
            _actionRunRepository = actionRunRepository;
        }

        public async Task ExecuteAsync(Domain.Agent agent, int maxIterations = 10)
        {            
            var systemPrompt = GenerateSystemPrompt(agent.Name, agent.Mission, agent.Capabilities);

            var test = new [] { "Email Report" };
            agent.Actions = agent.Actions.Where(x => test.Contains(x.Name)).ToList();
            //foreach (var action in agent.Actions)
            for (int ai = 0; ai < agent.Actions.Count; ai++)
            {
                _context.IsComplete = false;
                int iteration = 0;
                var actionFinalResult = "";
                byte[] byteData = null;

                while (!_context.IsComplete && iteration < maxIterations)
                {
                    iteration++; 

                    var configsForLLM = new RequestLLMDto();

                    configsForLLM.SystemPrompt = systemPrompt;

                    var toolResult = "";
                    byte[] toolResultByteData = null;

                    for (int ti = 0; ti < agent.Actions[ai].Tools.Count; ti++)
                    {
                        var nextToolName = ti < agent.Actions[ai].Tools.Count - 1 ? agent.Actions[ai].Tools[ti + 1].Name : null;
 
                        //get result from previous Action
                        if(ai > 0)
                        {
                            var prevAction = agent.Actions[ai - 1];
                            if(prevAction.Tools.Count > 0 && prevAction.Tools.Last().Name == "pdf-generator")
                            {
                                toolResultByteData = await _actionRunRepository.GetActionRunByteDataByNameAndAgentNameAsync(agent.Name, prevAction.Name);
                                if(toolResultByteData != null)
                                {
                                    toolResult = "Result is a PDF in byte array format. Don't prepare any input, leave it empty. Set stricly completed to true";
                                }
                                
                            } else
                            {
                                var data = await _actionRunRepository.GetActionRunDataByNameAndAgentNameAsync(agent.Name, prevAction.Name);
                                toolResult = data;
                            }
                        }

                        //for test
                        //var data = await _actionRunRepository.GetActionRunByNameAndAgentNameAsync(agent.Name, "Design HTML and Generate PDF Report");
                        toolResultByteData = await _actionRunRepository.GetActionRunByteDataByNameAndAgentNameAsync(agent.Name, "Design HTML and Generate PDF Report");
                        toolResult = "Result is a PDF in byte array format. Don't prepare any input, leave it empty. Set stricly completed to true";

                        configsForLLM.UserPrompt = PrepareToolInputPrompt(agent.Actions[ai], toolResult, agent.Actions[ai].Tools[ti].Name, nextToolName);

                        ResponseLLMDto llmResponse = null;

                        if (agent.Actions[ai].Tools[ti].Name == "pdf-generator")
                        {
                            configsForLLM.MaxTokens = 2000;
                            llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM);
                        } else
                        {
                            llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM);
                        }

                        var toolCompleted = false;
                        dynamic input = llmResponse.Input;

                        while (!toolCompleted && iteration < maxIterations)
                        {
                            if (_tools.TryGetValue(agent.Actions[ai].Tools[ti].Name, out var tool))
                            {
                                var output = "";
                                if (agent.Actions[ai].Tools[ti].Name == "data-analyzer")
                                {

                                    var dictionary = new Dictionary<string, object>();
                                    dictionary.Add("Name", agent.Actions[ai].Name);
                                    dictionary.Add("Description", agent.Actions[ai].Description);
                                    dictionary.Add("ActionToExecute", agent.Actions[ai].ActionToExecute); 
                                    dictionary.Add("Data", toolResult);

                                    var toolConfig = new ToolConfigDto { Inputs = dictionary };
                                    var result = await tool.ExecuteAsync(toolConfig);
                                    output = result.Data;

                                } else if (agent.Actions[ai].Tools[ti].Name == "gmail-client")
                                {
                                    
                                    var dictionary = new Dictionary<string, object>();
                                    dictionary.Add("Pdf", toolResultByteData);
                                    dictionary.Add("PdfName", "TestPdf");
                                    dictionary.Add("RecipientEmail", agent.Actions[ai].Tools[ti].RecipientEmail); 
                                    dictionary.Add("RecipientName", agent.Actions[ai].Tools[ti].RecipientName);

                                    var toolConfig = new ToolConfigDto { Inputs = dictionary };
                                    var result = await tool.ExecuteAsync(toolConfig);
                                    output = result.Data;

                                } else
                                {
                                    var toolConfig = new ToolConfigDto { Input = input };
                                    var result = await tool.ExecuteAsync(toolConfig);
                                    
                                    
                                    if(result != null && 
                                       result.DataType == Contracts.Enums.ToolDataTypeEnum.Byte && 
                                       result.ByteData != null)
                                    {
                                        byteData = result.ByteData;

                                    } else if (result != null && result.DataType == Contracts.Enums.ToolDataTypeEnum.String)
                                    {
                                        output = result.Data;
                                    }
                                    
                                }

                                toolResult = output;
                                toolCompleted = true;
 
                            }
                            else
                            {
                                // Handle unrecognized tool or incomplete response
                                Console.WriteLine($"Tool '{agent.Actions[ai].Tools[ti].Name}' not found or invalid instruction.");
                                break;
                            }
                        }
                    }

                    actionFinalResult = toolResult;
                    if (actionFinalResult != null || !string.IsNullOrEmpty(actionFinalResult)) 
                    {
                        await _actionRunRepository.SaveActionRunAsync(agent.Name, agent.Actions[ai].Name, actionFinalResult);
                    }

                    if (byteData != null)
                    {
                        await _actionRunRepository.SaveActionRunForByteDataAsync(agent.Name, agent.Actions[ai].Name, byteData);
                    }
                    
                    
                    _context.Data.Add(agent.Actions[ai].Name, actionFinalResult);
                    _context.IsComplete = true;
                }

                if (!_context.IsComplete)
                {
                    Console.WriteLine("Action did not complete within the iteration limit.");
                }
            }

            Console.WriteLine("Operation finished");
        }

        private string PrepareToolInputPrompt(Domain.Action action, string toolResult, string toolName, string? nextToolName)
        {
            // Retrieve the tool configuration for the current tool
            var toolConfig = action.Tools.FirstOrDefault(t => t.Name == toolName);

            // Check if the toolConfig exists and get the expected input type
            var expectedInputType = toolConfig?.Input ?? "text"; // Default to "text" if not specified

            // Adjust the prompt based on the expected input type
            string inputInstruction = expectedInputType switch
            {
                "url" => "The tool expects input as a single URL or a list of URLs. Strictly provide only URLs in the required format.",
                "text" => "The tool expects input in plain text format. Prepare the necessary input accordingly.",
                "json" => "The tool expects input as a JSON object. Provide the required data in JSON format.",
                "html" => "The tool expects input as a HTML object. Provide the required data in HTML format.",
                "pdf" => "The tool expects input PDF as a byte array. Provide the required data in byte array format.",
                _ => toolConfig?.Input
            };

            // Return the prompt with the dynamic instruction
            return $@"
                You are tasked with executing an action using a sequence of tools.

                Action: {action.Name}
                Current Tool: {toolName}
                Next Tool: {(nextToolName ?? "None")}

                Tool Result So Far: {toolResult}

                {inputInstruction}

                Strictly adhere to the following response format:
                {{
                  ""input"": ""appropriate format based on the tool's requirements. Stricly avoid JSON in your answer"",
                  ""completed"": false
                }}

                Generate the necessary input for the current tool to proceed.";
        }


        private string AnalyzeToolOutputPrompt(string toolResult, string? nextToolName)
        {
            return $@"
                You received the following output from the tool:

                {toolResult}

                Analyze this result and decide whether it is sufficient to proceed to the next tool ({nextToolName ?? "None"}).

                Strictly adhere to the following response format:
                {{
                  ""output"": ""blank for analysis"",
                  ""completed"": boolean true or false
                }}

                Ensure that the completed field indicates whether the action is done. If one more iteration is needed then set to false.";
        }

        private string GenerateSystemPrompt(string role, string mission, List<Capability> capabilities)
        {
            var capabilitiesList = string.Join(", ", capabilities.Select((cap, index) => $"{index + 1}. {cap.Task}: {cap.Description}"));

            return $@"You are an intelligent assistant with the following role: {role}.

                Mission: {mission}
                Capabilities: {capabilitiesList}

                Follow instructions carefully and execute actions using the tools provided.";
        }

        
    }
}
