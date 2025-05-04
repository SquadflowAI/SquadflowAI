using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts.Dtos
{
    public class UIFlowDto
    {
        public Guid? Id { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Name { get; set; }
        public IEnumerable<UIAgentNodeDto>? Nodes {get; set;}
        public IEnumerable<UIAgentNodeConnectionDto>? Connections {get;set;}
    }
}
