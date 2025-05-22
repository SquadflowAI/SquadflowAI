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
        public UIFlowService(IUIFlowRepository uIFlowRepository, IFlowExecutorService flowExecutorService)
        {
            _uIFlowRepository = uIFlowRepository;
            _flowExecutorService = flowExecutorService;
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

            return result;
        }

        public async Task DeleteUIFlowByIdAsync(Guid id)
        {
            await _uIFlowRepository.DeleteUIFlowByIdAsync(id);
        }
    }
}
