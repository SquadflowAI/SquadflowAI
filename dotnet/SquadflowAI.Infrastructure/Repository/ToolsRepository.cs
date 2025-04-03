using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.Tools;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class ToolsRepository : IToolsRepository
    {
        private readonly DbContext _dbContext;
        public ToolsRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ToolDto>> GetToolsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var toolsQuery = @"
            SELECT *
            FROM tools";

            IEnumerable<ToolDto> toolsQueryResult = await connection.QueryAsync<ToolDto>(toolsQuery);

            if (toolsQueryResult == null)
                return null;

            //var result = JsonConvert.DeserializeObject<IEnumerable<ToolDto>>(toolsQueryResult);

            return toolsQueryResult;
        }
    }
}
