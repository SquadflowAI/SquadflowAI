using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IAgentRepository
    {
        Task CreateAgentAsync(Agent agent);

        Task<Agent> GetAgentByNameAsync(string name);
    }
}
