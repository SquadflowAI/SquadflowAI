using SquadflowAI.Contracts.User;
using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IUserService
    {
        Task CreateUserAsync(CreateUserDto user);
        Task<User> GetUserByIdAsync(Guid id);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login);
    }
}
