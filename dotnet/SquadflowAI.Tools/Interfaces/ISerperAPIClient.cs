using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Tools.Interfaces
{
    public interface ISerperAPIClient
    {
        Task<string> MakeRequestAsync(string query);
    }
}
