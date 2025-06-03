using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts.Tools
{
    public class SerperAPIResponseDto
    {
        public IEnumerable<SerperAPIOrganicDto> Organic { get; set; }
    }

    public class SerperAPIOrganicDto
    {
        public string Link { get; set; }
        public int Position { get; set; }
    }
}
