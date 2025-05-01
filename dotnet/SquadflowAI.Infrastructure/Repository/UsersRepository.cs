using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DbContext _dbContext;
        public UsersRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateUserAsync(User user)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var userQuery = "INSERT INTO users (name, email, password, createdDate, updatedDate) VALUES (@name, @email, @password, @createdDate, @updatedDate)";
            await connection.ExecuteAsync(userQuery, new { user.Name, user.Email, user.Password, user.CreatedDate, user.UpdatedDate });
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var userQuery = @"
            SELECT *
            FROM users a
            WHERE a.id = @id";

            User userQueryResult = await connection.QuerySingleOrDefaultAsync<User>(userQuery, new { id = id });

            if (userQueryResult == null)
                return null;

            return userQueryResult;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var userQuery = @"
            SELECT *
            FROM users a
            WHERE a.email = @email";

            User userQueryResult = await connection.QuerySingleOrDefaultAsync<User>(userQuery, new { email = email });

            if (userQueryResult == null)
                return null;

            return userQueryResult;
        }


        
    }
}
