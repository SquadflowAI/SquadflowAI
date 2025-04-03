using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class UIOrchestrationRepository : IUIOrchestrationRepository
    {
        private readonly DbContext _dbContext;
        public UIOrchestrationRepository(DbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task CreateUIOrchestrationAsync(UIOrchestrationDto uiorchestration)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var data = JsonConvert.SerializeObject(uiorchestration);
            var uiorchestrationQuery = "INSERT INTO uiorchestration (name, data) VALUES (@name, @data::jsonb)";
            await connection.ExecuteAsync(uiorchestrationQuery, new { uiorchestration.Name, data });
        }

        public async Task<UIOrchestrationDto> GetUIOrchestrationByNameAsync(string inputName)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiorchestrationQuery = @"
            SELECT a.data
            FROM uiorchestration a
            WHERE a.name = @inputName";

            string uiorchestrationQueryResult = await connection.QuerySingleOrDefaultAsync<string>(uiorchestrationQuery, new { inputName = inputName });

            if (uiorchestrationQueryResult == null)
                return null;

            var result = JsonConvert.DeserializeObject<UIOrchestrationDto>(uiorchestrationQueryResult);

            return result;
        }

        public async Task<IEnumerable<UIOrchestrationDto>> GetUIOrchestrationsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiorchestrationQuery = @"
            SELECT a.data
            FROM uiorchestration a";

            string uiorchestrationQueryResult = await connection.QuerySingleOrDefaultAsync<string>(uiorchestrationQuery);

            if (uiorchestrationQueryResult == null)
                return null;

            var result = JsonConvert.DeserializeObject<IEnumerable<UIOrchestrationDto>>(uiorchestrationQueryResult);

            return result;
        }
    }
}
