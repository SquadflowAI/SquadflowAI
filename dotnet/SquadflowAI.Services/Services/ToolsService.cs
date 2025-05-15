using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.Tools;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Services
{
    public class ToolsService : IToolsService
    {
        private readonly IToolsRepository _toolsRepository;
        public ToolsService(IToolsRepository toolsRepository)
        {
            _toolsRepository = toolsRepository;
        }
        public async Task<IEnumerable<ToolDto>> GetToolsAsync()
        {
            var result = await _toolsRepository.GetToolsAsync();

            return result;
        }

        public async Task<IEnumerable<CoreToolDto>> GetCoreToolsAsync()
        {
            var result = await _toolsRepository.GetCoreToolsAsync();

            return result;
        }

        public async Task<IEnumerable<AIToolDto>> GetAIToolsAsync()
        {
            var result = await _toolsRepository.GetAIToolsAsync();

            return result;
        }

        public async Task<IEnumerable<AppDto>> GetAppsAsync()
        {
            var result = await _toolsRepository.GetAppsAsync();

            return result;
        }
    }
}
