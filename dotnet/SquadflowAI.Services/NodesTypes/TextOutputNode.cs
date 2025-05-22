using SquadflowAI.Contracts.Dtos;
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
        public string Id { get; private set; }

        public TextOutputNode(string id)
        {
            Id = id;
        }

        public Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {

            //

            return Task.FromResult(input);
        }
    }
}
