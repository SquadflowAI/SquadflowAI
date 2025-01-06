using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class AgentRepository : IAgentRepository
    {
        public AgentRepository() { }

        public async Task CreateAgentAsync(Agent agent)
        {

        }

        public async Task<Agent> GetAgentByNameAsync(string agentName)
        {
            return null;
        }
    }


}
