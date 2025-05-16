using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class AISummarizeTextNode : INode
    {
        public string Id { get; private set; }

        public AISummarizeTextNode(string id) { Id = id; }

        public Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters)
        {
            var instruction = parameters["instruction"];

            // Call your LLM API here 

            return Task.FromResult($"[Summary of '{input}'] using instruction: {instruction}");
        }
    }
}
