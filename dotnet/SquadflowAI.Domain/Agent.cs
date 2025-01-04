using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Domain
{
    public class Agent
    {
        public string Role { get; set; }
        public string Goal { get; set; }
        public string LLM { get; set; }
        public IEnumerable<string> Tools { get; set; }
    }
}
