using SquadflowAI.Contracts.Dtos;
using SquadflowAI.LLMConnector.Interfaces;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class TextInputNode : INode
    {

        public string Id { get; private set; }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]>? parametersByte = null)
        {
            var param = parameters["text"];
            return Task.FromResult(param);
        }
    }
}
