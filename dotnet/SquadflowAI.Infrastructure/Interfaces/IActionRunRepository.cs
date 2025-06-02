using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IActionRunRepository
    {
        Task CreateActionRunAsync(Guid? agentId, Guid? flowId, string data);
        Task CreateActionRunForByteDataAsync(Guid? agentId, Guid? flowId, byte[] bytedata);
        Task<IEnumerable<ActionRun>> GetActionRunsByAgentIdAsync(Guid agentId);
        Task<IEnumerable<ActionRun>> GetActionRunsByFlowIdAsync(Guid flowId);


        Task<string> GetActionRunDataByNameAndAgentNameAsyncOBSOLETE(string agent, string actionName);
        Task<byte[]> GetActionRunByteDataByNameAndAgentNameAsyncOBSOLETE(string agent, string actionName);


    }
}
