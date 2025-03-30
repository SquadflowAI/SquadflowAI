using SquadflowAI.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IUIOrchestrationService
    {
        Task CreateUIOrchestrationAsync(UIOrchestrationDto uiorchestration);

        Task<UIOrchestrationDto> GetUIOrchestrationByNameAsync(string name);

        Task<IEnumerable<UIOrchestrationDto>> GetUIOrchestrationsAsync();
    }
}
