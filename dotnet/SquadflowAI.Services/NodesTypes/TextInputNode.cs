using SquadflowAI.Contracts;
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

        public async Task<ExecutionInputOutputDto> ExecuteAsync(ExecutionInputOutputDto input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]>? parametersByte = null)
        {
            var output = new ExecutionInputOutputDto();

            var param = parameters["text"];

            output.Input = param;
            return output;
        }
    }
}
