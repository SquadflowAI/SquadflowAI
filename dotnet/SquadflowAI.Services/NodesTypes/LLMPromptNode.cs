using SquadflowAI.Contracts.Dtos;
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
        public string Id { get; private set; }

        public LLMPromptNode(string id) { Id = id; }  

        public Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {
            var prompt = parameters["prompt"].Replace("{{input}}", input);

            //LLM API here
            var result = "AAPL has the price of xxx today";

            return Task.FromResult($"[LLM processed]: {result}");
        }
    }
}
