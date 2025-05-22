using SquadflowAI.Contracts.Dtos;
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

        private readonly string _staticText;

        public TextInputNode(string id, string staticText)
        {
            Id = id;
            _staticText = staticText;
        }

        public Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, UIFlowDto uIFlow)
        {
            return Task.FromResult(_staticText);
        }
    }
}
