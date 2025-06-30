using SquadflowAI.Contracts;
using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IFlowTemplatesRepository
    {
        Task<IEnumerable<FlowTemplate>> GetFlowTemplatesAsync();
    }
}
