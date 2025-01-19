using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class RequestLLMDto
    {
        public string SystemPrompt {  get; set; }
        public string UserPrompt { get; set; }
    }
}
