using SquadflowAI.Contracts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.NodesTypes.Base
{
    public interface INode
    {
        string Id { get; }
        Task<string> ExecuteAsync(string input, IDictionary<string, string> parameters, 
            UIFlowDto uIFlow, IDictionary<string, byte[]>? parametersByte = null);
    }
}
