using SquadflowAI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IIntegrationsService
    {
        Task CreateIntegrationAsync(IntegrationsDto integration);
        Task<IntegrationsDto> GetIntegrationByUserIdAsync(Guid id);
    }
}
