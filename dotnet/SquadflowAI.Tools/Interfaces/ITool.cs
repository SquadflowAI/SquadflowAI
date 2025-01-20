using SquadflowAI.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.Interfaces
{
    public interface ITool
    {
        string Key { get; }
        public Task<string> ExecuteAsync(ToolConfigDto config);
 
    }
}
