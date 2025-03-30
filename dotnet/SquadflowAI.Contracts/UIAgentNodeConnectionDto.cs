using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts.Dtos
{
    public class UIAgentNodeConnectionDto
    {
        public string? Id {get;set;}
        public string? Name {get;set;}
        public string? SourceNodeId {get;set;}
        public string? TargetNodeId {get;set;}
        public UIAgentNodeDto? InputAgentNode { get; set; }
        public UIAgentNodeDto? OutputAgentNode { get; set; }

    }
}
