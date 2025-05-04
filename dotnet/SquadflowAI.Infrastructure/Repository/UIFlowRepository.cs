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
            var uiflowQuery = "INSERT INTO uiflows (name, projectId, data) VALUES (@name, @projectId, @data::jsonb)";

            await connection.ExecuteAsync(uiflowQuery, new { flow.Name, flow.ProjectId, data });
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

            IEnumerable<UIFlowDto> uiflowQueryResult = await connection.QueryAsync<UIFlowDto>(uiflowQuery);

            if (uiflowQueryResult == null)
                return null;

            return uiflowQueryResult.ToList();
        }

        public async Task<IEnumerable<UIFlowDto>> GetUIFlowsByProjectIdAsync(Guid projectId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT *
            FROM uiflows a WHERE a.projectId = @projectId";

            IEnumerable<UIFlowDto> projectsQueryResult = await connection.QueryAsync<UIFlowDto>(uiflowQuery, new { projectId = projectId });

            if (projectsQueryResult == null)
                return null;

            return projectsQueryResult.ToList();
        }
    }
}
