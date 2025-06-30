using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class UIAgentNodeConnection
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? SourceNodeId { get; set; }
        public string? TargetNodeId { get; set; }
        public UIAgentNode? InputAgentNode { get; set; }
        public UIAgentNode? OutputAgentNode { get; set; }
    }
}
