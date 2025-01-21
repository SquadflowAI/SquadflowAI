using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IActionRunRepository
    {
        Task SaveActionRunAsync(string agentName, string actionName, string data);

        Task<string> GetActionRunByNameAndAgentNameAsync(string agent, string actionName);
    }
}
