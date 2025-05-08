using SquadflowAI.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IUIFlowRepository
    {
        Task CreateUIFlowAsync(UIFlowDto flow);
        Task UpdateUIFlowAsync(UIFlowDto flow);
        Task<UIFlowDto> GetUIFlowByNameAsync(string name);
        Task<UIFlowDto> GetUIFlowByIdAsync(Guid id);
        Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync();
        Task<IEnumerable<UIFlowDto>> GetUIFlowsByProjectIdAsync(Guid projectId);
    }
}
