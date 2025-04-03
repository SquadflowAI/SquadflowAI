using SquadflowAI.Contracts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IToolsService
    {
        Task<IEnumerable<ToolDto>> GetToolsAsync();
    }
}
