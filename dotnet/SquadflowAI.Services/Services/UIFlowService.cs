using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Services
{
    public class UIFlowService : IUIFlowService
    {
        private readonly IUIFlowRepository _uIFlowRepository;
        private readonly IFlowExecutorService _flowExecutorService;
        private IActionRunRepository _actionRunRepository;

        public UIFlowService(IUIFlowRepository uIFlowRepository, 
            IFlowExecutorService flowExecutorService,
            IActionRunRepository actionRunRepository)
        {
            _uIFlowRepository = uIFlowRepository;
            _flowExecutorService = flowExecutorService;
            _actionRunRepository = actionRunRepository;
        }

        public async Task CreateUIFlowAsync(UIFlowDto uiflow)
        {
            await _uIFlowRepository.CreateUIFlowAsync(uiflow);
        }

        public async Task UpdateUIFlowAsync(UIFlowDto uiflow)
        {
            await _uIFlowRepository.UpdateUIFlowAsync(uiflow);
        }

        public async Task<UIFlowDto> GetUIFlowByNameAsync(string name)
        {
            var result = await _uIFlowRepository.GetUIFlowByNameAsync(name);

            return result;
        }

        public async Task<UIFlowDto> GetUIFlowByIdAsync(Guid id)
        {
            var result = await _uIFlowRepository.GetUIFlowByIdAsync(id);

            return result;
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync()
        {
            var result = await _uIFlowRepository.GetUIFlowsAsync();

            return result;
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsByProjectIdAsync(Guid projectId)
        {
            var result = await _uIFlowRepository.GetUIFlowsByProjectIdAsync(projectId);

            return result;
        }

        public async Task<string> RunUIFlowByIdAsync(Guid id)
        {
            var flow = await _uIFlowRepository.GetUIFlowByIdAsync(id);

            var result = await _flowExecutorService.ExecuteAsync(flow);

            if (result != null)
            {
                await _actionRunRepository.CreateActionRunAsync(null, flow.Id, result);
            }

            return result;
        }

        public async Task DeleteUIFlowByIdAsync(Guid id)
        {
            await _uIFlowRepository.DeleteUIFlowByIdAsync(id);
        }

        #region ActionRuns

        public async Task<IEnumerable<ActionRun>> GetActionRunsByAgentIdAsync(Guid agentId)
        {
            var result = await _actionRunRepository.GetActionRunsByAgentIdAsync(agentId);

            return result;
        }

        public async Task<IEnumerable<ActionRun>> GetActionRunsByFlowIdAsync(Guid flowId)
        {
            var result = await _actionRunRepository.GetActionRunsByFlowIdAsync(flowId);

            return result;
        }

        #endregion
    }
}
