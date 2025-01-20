using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IActionRunRepository
    {
        Task SaveActionAsync(string agentName, string actionName, string data);
    }
}
