using SquadflowAI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IIntegrationsRepository
    {
        Task CreateIntegrationAsync(IntegrationsDto integration);
        Task<IntegrationsDto> GetIntegrationByUserIdAsync(Guid userId);
    }
}
