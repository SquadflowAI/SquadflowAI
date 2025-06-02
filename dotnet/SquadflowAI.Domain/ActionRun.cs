using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class ActionRun
    {
        public Guid? AgentId { get; set; }
        public Guid? FlowId { get; set; }
        public string CreatedDate { get; set; }
        public string Data { get; set; }
        public byte[] ByteData { get; set; }



    }
}
