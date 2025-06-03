using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.Interfaces
{
    public interface ISerperAPIClient
    {
        Task<SerperAPIResponseDto> ExecuteAsync(string query, string serperApiKey);
    }
}
