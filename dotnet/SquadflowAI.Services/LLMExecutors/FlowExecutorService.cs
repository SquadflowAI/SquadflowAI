using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.NodesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.LLMExecutors
{
    public class FlowExecutorService
    {
        private readonly Dictionary<int, UIAgentNodeDto> _flowMap;

        public FlowExecutorService(UIFlowDto uIFlow) 
        {
            _flowMap = uIFlow.Nodes.ToDictionary(n => n.OrderSequence);
        }

        public async Task<string> ExecuteAsync()
        {
            var currentNodeSequence = _flowMap.First().Key;
            string input = "";

            while (currentNodeSequence != 0)
            {
                var nodeData = _flowMap[currentNodeSequence];
                var node = NodeFactory.CreateNode(nodeData);
                input = await node.ExecuteAsync(input, nodeData.Parameters);

                currentNodeSequence = nodeData.NextNodeIds.FirstOrDefault();
            }

            return input;
        }
    }
}
