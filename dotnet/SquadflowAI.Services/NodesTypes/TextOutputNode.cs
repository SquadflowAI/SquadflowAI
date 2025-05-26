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

        public async Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {

            var flow = await _uIFlowRepository.GetUIFlowByIdAsync((Guid)uIFlow.Id);

            if (flow != null)
            {
                var textOutputNode = flow.Nodes.SingleOrDefault(x => x.Type == Type);
                if (textOutputNode != null) 
                {
                    textOutputNode.Output = input;

                    await _uIFlowRepository.UpdateUIFlowAsync(flow);
                }
            }

            return input;
        }
    }
}
