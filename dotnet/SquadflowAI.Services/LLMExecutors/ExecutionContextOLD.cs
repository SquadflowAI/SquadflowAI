using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.LLMExecutors
{
    public class ExecutionContextOLD
    {
        public string Mission { get; set; } // Agent's mission
        public bool IsComplete { get; set; } // Workflow completion flag
        public Dictionary<string, object> Data { get; } = new(); // Shared data store
    }
 
}
