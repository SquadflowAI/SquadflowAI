using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Services.NodesTypes.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes
{
    public class TextOutputNode : INode
    {
        private string Type = "text-output";
        public string Id { get; private set; }

        private readonly IUIFlowRepository _uIFlowRepository;
        public TextOutputNode(IUIFlowRepository uIFlowRepository)
        {
            _uIFlowRepository = uIFlowRepository;
        }

        public void Initialize(string id, IDictionary<string, string> parameters)
        {
            Id = id;
        }

        public async Task<ExecutionInputOutputDto> ExecuteAsync(ExecutionInputOutputDto input, IDictionary<string, string> parameters, UIFlowDto uIFlow, IDictionary<string, byte[]>? parametersByte = null)
        {
            var output = new ExecutionInputOutputDto();

            var flow = await _uIFlowRepository.GetUIFlowByIdAsync((Guid)uIFlow.Id);

            if (flow != null)
            {
                var textOutputNode = flow.Nodes.SingleOrDefault(x => x.Type == Type);
                if (textOutputNode != null) 
                {
                    textOutputNode.Output = input.Input;

                    await _uIFlowRepository.UpdateUIFlowAsync(flow);
                }
            }

            output.Input = input.Input;
            return output;
        }
    }
}
