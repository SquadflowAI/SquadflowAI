﻿using Newtonsoft.Json;
using SquadflowAI.Contracts;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.LLMConnector.OpenAI;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Tools;
using SquadflowAI.Tools.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.LLMExecutors
{
    public class OpenAILLMExecutorService : IOpenAILLMExecutorService
    {
        private readonly ExecutionContext _context;
        private IOpenAIAPIClient _openAIAPIClient;
        private readonly Dictionary<string, ITool> _tools;
        public OpenAILLMExecutorService(IOpenAIAPIClient openAIAPIClient, IEnumerable<ITool> tools) 
        {
            _openAIAPIClient = openAIAPIClient;
            _context = new ExecutionContext();
            _tools = tools.ToDictionary(strategy => strategy.Key);
        }

        public async Task ExecuteAsync(Domain.Agent agent, int maxIterations = 10)
        {            
            var systemPrompt = GenerateSystemPrompt(agent.Name, agent.Mission, agent.Capabilities);

            foreach (var action in agent.Actions) 
            {
                _context.IsComplete = false;
                int iteration = 0;

                while (!_context.IsComplete && iteration < maxIterations)
                {
                    iteration++; 

                    var configsForLLM = new RequestLLMDto();

                    configsForLLM.SystemPrompt = systemPrompt;
 
                    var toolResult = "";

                    for (int i = 0; i < action.Tools.Count; i++)
                    {
                        var nextToolName = i < action.Tools.Count - 1 ? action.Tools[i + 1].Name : null;
 
                        configsForLLM.UserPrompt = PrepareToolInputPrompt(action, toolResult, action.Tools[i].Name, nextToolName);

                        var llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM);

                        var toolCompleted = false; //llmResponse.Completed;
                        dynamic input = llmResponse.Input;

                        while (!toolCompleted && iteration < maxIterations)
                        {
                            if (_tools.TryGetValue(action.Tools[i].Name, out var tool))
                            {

                                var toolConfig = new ToolConfigDto { Input = input };
                                var output = await tool.ExecuteAsync(toolConfig);

                                toolResult = output;
                                toolCompleted = true;
                                //configsForLLM.UserPrompt = AnalyzeToolOutputPrompt(toolResult, nextToolName);

                                //llmResponse = await _openAIAPIClient.SendMessageAsync(configsForLLM);

                                //if (llmResponse.Completed)
                                //{
                                //    toolCompleted = true;  
                                //}
                                //else
                                //{
                                //    iteration++;
                                //    if (iteration >= maxIterations)
                                //    {
                                //        Console.WriteLine("Maximum iterations reached for this tool.");
                                //        break; // Exit inner loop due to iteration limit
                                //    }
                                //}
                            }
                            else
                            {
                                // Handle unrecognized tool or incomplete response
                                Console.WriteLine($"Tool '{action.Tools[i].Name}' not found or invalid instruction.");
                                break;
                            }
                        }
                    }

                    _context.IsComplete = true;
                }

                if (!_context.IsComplete)
                {
                    Console.WriteLine("Action did not complete within the iteration limit.");
                }
            }

            Console.WriteLine("Operation finished");
        }



        

        //private string PrepareToolInputPrompt(Domain.Action action, string toolResult, string toolName, string? nextToolName)
        //{
        //    return $@"
        //        You are tasked with executing an action using a sequence of tools.

        //        Action: {action.Name}
        //        Current Tool: {toolName}
        //        Next Tool: {(nextToolName ?? "None")}

        //        Tool Result So Far: {toolResult}

        //        Strictly adhere to the following response format:
        //        {{
        //          ""input"": ""Provide the necessary input for the tool as a string or JSON object"",
        //          ""completed"": false
        //        }}

        //        Prepare the necessary input for the current tool to proceed.";
        //}

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
                  ""input"": <appropriate format based on the tool's requirements>,
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

                Ensure that the completed field indicates whether the action is done. If one more iteration needed set to false.";
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