using Dapper;
using SquadflowAI.Contracts;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class IntegrationsRepository : IIntegrationsRepository
    {
        private readonly DbContext _dbContext;
        public IntegrationsRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateIntegrationAsync(IntegrationsDto integration)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var now = DateTime.UtcNow;

            var integrationQuery = "INSERT INTO integrations (userId, openAIKey) VALUES (@userId, @openAIKey)";
            await connection.ExecuteAsync(integrationQuery, new { integration.UserId, integration.OpenAIKey });
        }

        public async Task<IntegrationsDto> GetIntegrationByUserIdAsync(Guid userId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var userQuery = @"
            SELECT *
            FROM integrations a
            WHERE a.userId = @userId";

            IntegrationsDto integrationsQueryResult = await connection.QuerySingleOrDefaultAsync<IntegrationsDto>(userQuery, new { userId = userId });

            if (integrationsQueryResult == null)
                return null;

            return integrationsQueryResult;
        }
    }
}
