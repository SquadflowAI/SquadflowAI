using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Infrastructure.Interfaces;
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
        public UIFlowService(IUIFlowRepository uIFlowRepository)
        {
            _uIFlowRepository = uIFlowRepository;
        }

        public async Task CreateUIFlowAsync(UIFlowDto uiflow)
        {
            await _uIFlowRepository.CreateUIFlowAsync(uiflow);
        }

        public async Task<UIFlowDto> GetUIFlowByNameAsync(string name)
        {
            var result = await _uIFlowRepository.GetUIFlowByNameAsync(name);

            return result;
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync()
        {
            var result = await _uIFlowRepository.GetUIFlowsAsync();

            return result;
        }
    }
}
