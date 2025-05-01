using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Contracts.User
{
    public class LoginRequestDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
