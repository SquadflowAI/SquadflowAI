using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts
{
    public class IntegrationsDto
    {
        public Guid? UserId { get; set; }
        public string? OpenAIKey { get; set; }
        public string? SerperAPIKey { get; set; }

    }
}
