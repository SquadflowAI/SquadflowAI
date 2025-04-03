using SquadflowAI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.LLMConnector.Interfaces
{
    public interface IOpenAIAPIClient
    {
        Task<ResponseLLMDto> SendMessageAsync(RequestLLMDto request);
    }
}
