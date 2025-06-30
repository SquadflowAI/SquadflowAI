using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class FlowTemplate
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Data { get; set; }
        public UIFlow? DataConverted { get; set; }

        //public IEnumerable<UIAgentNode>? Nodes { get; set; }
        //public IEnumerable<UIAgentNodeConnection>? Connections { get; set; }
    }
}
