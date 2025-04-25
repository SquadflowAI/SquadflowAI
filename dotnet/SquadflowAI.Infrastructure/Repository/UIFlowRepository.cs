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
    public class UIFlowRepository : IUIFlowRepository
    {
        private readonly DbContext _dbContext;
        public UIFlowRepository(DbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task CreateUIFlowAsync(UIFlowDto flow)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var data = JsonConvert.SerializeObject(flow);
            var uiflowQuery = "INSERT INTO uiflows (name, data) VALUES (@name, @data::jsonb)";
            await connection.ExecuteAsync(uiflowQuery, new { flow.Name, data });
        }

        public async Task<UIFlowDto> GetUIFlowByNameAsync(string inputName)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT a.data
            FROM uiflows a
            WHERE a.name = @inputName";

            string uiflowQueryResult = await connection.QuerySingleOrDefaultAsync<string>(uiflowQuery, new { inputName = inputName });

            if (uiflowQueryResult == null)
                return null;

            var result = JsonConvert.DeserializeObject<UIFlowDto>(uiflowQueryResult);

            return result;
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT a.data
            FROM uiflows a";

            IEnumerable<string> uiflowQueryResult = await connection.QueryAsync<string>(uiflowQuery);

            if (uiflowQueryResult == null)
                return null;

            List<UIFlowDto> finalResult = new List<UIFlowDto>();
            foreach (var item in uiflowQueryResult)
            {
                var result = JsonConvert.DeserializeObject<UIFlowDto>(item);

                finalResult.Add(result);
            }

            

            return finalResult;
        }
    }
}
