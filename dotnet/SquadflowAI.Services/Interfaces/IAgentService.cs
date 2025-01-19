using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IAgentService
    {
        Task CreateAgentAsync();

        Task<Domain.Agent> GetAgentByNameAsync(string agentName);
    }
}
