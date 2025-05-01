using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.User;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
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
        private string SecretKey = ""; // Replace with a securely stored key
        private const int TokenExpirationMinutes = 60; // Token expiration time
        private readonly IUsersRepository _userRepository;
        public UserService(IUsersRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;

            SecretKey = configuration.GetValue<string>("SERPER_API_KEY");
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

            var hashedPassword = HashPassword(user.Password);
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
            if (user == null || !VerifyPassword(login.Password, user.Password))
            {
                throw new Exception("Unauthorized");
            }

            var token = GenerateToken(user.Id.ToString());

            return new LoginResponseDto() { UserId = user.Id.ToString(), Token = token };
        }

        public string HashPassword(string password, int saltSize = 16, int hashSize = 32, int iterations = 3, int memorySize = 8192, int parallelism = 1)
        {
            byte[] salt = GenerateSalt(saltSize);

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = parallelism; // number of threads
                argon2.MemorySize = memorySize;           // in KB
                argon2.Iterations = iterations;           // number of iterations

                byte[] hash = argon2.GetBytes(hashSize);
                return $"{Convert.ToBase64String(hash)}:{Convert.ToBase64String(salt)}";
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            string[] parts = hashedPassword.Split(':');
            byte[] hash = Convert.FromBase64String(parts[0]);
            byte[] salt = Convert.FromBase64String(parts[1]);

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = 1;
                argon2.MemorySize = 8192;
                argon2.Iterations = 3;

                byte[] computedHash = argon2.GetBytes(hash.Length);
                return CryptographicOperations.FixedTimeEquals(computedHash, hash);
            }
        }

        private byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public string GenerateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            // Define token descriptor with claims and expiration
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(TokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create and write the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
