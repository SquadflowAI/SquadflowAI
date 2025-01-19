using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class ResponseLLMDto
    {
        public dynamic Input { get; set; }
        public dynamic Output { get; set; }
        public bool Completed { get; set; }
    }
}
