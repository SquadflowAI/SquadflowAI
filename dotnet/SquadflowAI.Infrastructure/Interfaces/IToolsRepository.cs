using SquadflowAI.Contracts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IToolsRepository
    {
        Task<IEnumerable<ToolDto>> GetToolsAsync();
        Task<IEnumerable<CoreToolDto>> GetCoreToolsAsync();
        Task<IEnumerable<AIToolDto>> GetAIToolsAsync();
        Task<IEnumerable<AppDto>> GetAppsAsync();
    }
}
