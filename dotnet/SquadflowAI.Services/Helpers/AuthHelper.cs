using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Helpers
{
    public static class AuthHelper
    {
        public static string SecretKey { get; set; }  // Replace with a securely stored key
        private const int TokenExpirationMinutes = 60; // Token expiration time

        public static void Initialize(IConfiguration configuration)
        {
            var key = configuration.GetValue<string>("SECRET");
            SecretKey = key;
        }

        public static string HashPassword(string password, int saltSize = 16, int hashSize = 32, int iterations = 3, int memorySize = 8192, int parallelism = 1)
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

        public static bool VerifyPassword(string password, string hashedPassword)
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

        private static byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static string GenerateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (string.IsNullOrEmpty(SecretKey))
            {
                throw new InvalidOperationException("SecretKey is not set.");
            }

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
