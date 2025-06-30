using Microsoft.AspNetCore.Http;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IUIFlowService
    {
        Task CreateUIFlowAsync(UIFlowDto flow);
        Task CreateUIFlowFromTeamplteAsync(FlowTemplate template, Guid userId, Guid projectId);
        Task UpdateUIFlowAsync(UIFlowDto uiflow);
        Task<UIFlowDto> GetUIFlowByNameAsync(string name);
        Task<UIFlowDto> GetUIFlowByIdAsync(Guid id);
        Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync();
        Task<IEnumerable<UIFlowDto>> GetUIFlowsByProjectIdAsync(Guid projectId);
        Task<string> RunUIFlowByIdAsync(Guid id);
        Task DeleteUIFlowByIdAsync(Guid id);

        #region ActionRun
        Task<IEnumerable<ActionRun>> GetActionRunsByAgentIdAsync(Guid agentId);
        Task<IEnumerable<ActionRun>> GetActionRunsByFlowIdAsync(Guid flowId);

        #endregion

        #region Download Upload
        Task InsertFileUIFlowAsync(Guid flowId, string nodeId, string key, IFormFile file);

        #endregion
    }
}
