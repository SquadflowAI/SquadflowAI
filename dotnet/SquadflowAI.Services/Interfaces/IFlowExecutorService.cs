using SquadflowAI.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IFlowExecutorService
    {
        Task<string> ExecuteAsync(UIFlowDto uIFlow);
    }
}
