using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts.Dtos
{
    public class UIAgentNodeDto
    {
        public string? Id { get; set; }
        public string? Name {get; set;}
        public int PositionX { get; set;}
        public int PositionY { get; set;}
    }
}
