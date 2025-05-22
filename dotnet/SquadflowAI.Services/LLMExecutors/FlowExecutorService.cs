using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.NodesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.LLMExecutors
{
    public class FlowExecutorService : IFlowExecutorService
    {
        public FlowExecutorService() 
        {
            
        }

        public async Task<string> ExecuteAsync(UIFlowDto uIFlow)
        {
            Dictionary<int, UIAgentNodeDto>  _flowMap = uIFlow.Nodes.ToDictionary(n => n.OrderSequence);

            var currentNodeSequence = _flowMap.First().Key;
            string input = "";

            while (currentNodeSequence != 0)
            {
                var nodeData = _flowMap[currentNodeSequence];
                var node = NodeFactory.CreateNode(nodeData);
                input = await node.ExecuteAsync(input, nodeData.Parameters, uIFlow);

                currentNodeSequence = nodeData.NextNodeIds.FirstOrDefault();
            }

            return input;
        }
    }
}
