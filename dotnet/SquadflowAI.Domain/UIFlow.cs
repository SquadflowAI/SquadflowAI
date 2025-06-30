using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class UIFlow
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Name { get; set; }
        public IEnumerable<UIAgentNode>? Nodes { get; set; }
        public IEnumerable<UIAgentNodeConnection>? Connections { get; set; }
    }
}
