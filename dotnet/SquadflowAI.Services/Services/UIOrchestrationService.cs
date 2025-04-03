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
    public class UIOrchestrationService : IUIOrchestrationService
    {
        private readonly IUIOrchestrationRepository _uIOrchestrationRepository;
        public UIOrchestrationService(IUIOrchestrationRepository uIOrchestrationRepository)
        {
            _uIOrchestrationRepository = uIOrchestrationRepository;
        }

        public async Task CreateUIOrchestrationAsync(UIOrchestrationDto uiorchestration)
        {
            await _uIOrchestrationRepository.CreateUIOrchestrationAsync(uiorchestration);
        }

        public async Task<UIOrchestrationDto> GetUIOrchestrationByNameAsync(string name)
        {
            var result = await _uIOrchestrationRepository.GetUIOrchestrationByNameAsync(name);

            return result;
        }

        public async Task<IEnumerable<UIOrchestrationDto>> GetUIOrchestrationsAsync()
        {
            var result = await _uIOrchestrationRepository.GetUIOrchestrationsAsync();

            return result;
        }
    }
}
