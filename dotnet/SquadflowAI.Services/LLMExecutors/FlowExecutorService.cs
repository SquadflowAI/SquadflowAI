using SquadflowAI.Contracts;
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
        private readonly NodeFactory _nodeFactory;
        public FlowExecutorService(NodeFactory nodeFactory) 
        {
            _nodeFactory = nodeFactory; 
        }

        public async Task<ExecutionInputOutputDto> ExecuteAsync(UIFlowDto uIFlow)
        {
            Dictionary<int, UIAgentNodeDto>  _flowMap = uIFlow.Nodes.ToDictionary(n => n.OrderSequence);

            var currentNodeSequence = _flowMap.First().Key;
            ExecutionInputOutputDto input = new ExecutionInputOutputDto();

            while (currentNodeSequence != 0)
            {
                var nodeData = _flowMap[currentNodeSequence];
                var node = _nodeFactory.CreateNode(nodeData);
                input = await node.ExecuteAsync(input, nodeData.Parameters, uIFlow, nodeData.ParametersByte);

                currentNodeSequence = nodeData.NextNodeIds.FirstOrDefault();
            }

            return input;
        }
    }
}
