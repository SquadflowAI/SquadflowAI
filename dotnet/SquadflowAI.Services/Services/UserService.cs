using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.User;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Services.Helpers;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Services
{
    public class UserService : IUserService
    {
       
        private readonly IUsersRepository _userRepository;
        public UserService(IUsersRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;

            AuthHelper.SecretKey = configuration.GetValue<string>("SECRET_KEY");
        }

        public async Task CreateUserAsync(CreateUserDto user)
        {
            if(user.Password != user.ConfirmPassword)
            {
                throw new Exception("Passwords don't match");
            }

            var newUser = new User();
            newUser.Name = user.Name;
            newUser.Email = user.Email;

            var hashedPassword = AuthHelper.HashPassword(user.Password);
            newUser.Password = hashedPassword;
            newUser.CreatedDate = DateTime.Now;
            newUser.UpdatedDate = DateTime.Now;

            await _userRepository.CreateUserAsync(newUser);
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var result = await _userRepository.GetUserByIdAsync(id);

            return result;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
        {
            var user = await _userRepository.GetUserByEmailAsync(login.Email);
            if (user == null || !AuthHelper.VerifyPassword(login.Password, user.Password))
            {
                throw new Exception("Unauthorized");
            }

            var token = AuthHelper.GenerateToken(user.Id.ToString());

            return new LoginResponseDto() { UserId = user.Id.ToString(), Token = token };
        }

       
    }
}
