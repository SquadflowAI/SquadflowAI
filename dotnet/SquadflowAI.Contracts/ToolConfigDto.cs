using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class ToolConfigDto
    {
        public dynamic Input { get; set; }

        public Dictionary<string, dynamic> Inputs { get; set; }
        public Dictionary<string, dynamic> Outputs { get; set; }
    }
}
