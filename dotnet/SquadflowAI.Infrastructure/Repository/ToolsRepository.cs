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
            FROM toolssystem";

            IEnumerable<ToolDto> toolsQueryResult = await connection.QueryAsync<ToolDto>(toolsQuery);

            if (toolsQueryResult == null)
                return null;

            return toolsQueryResult;
        }

        public async Task<IEnumerable<CoreToolDto>> GetCoreToolsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var toolsQuery = @"
            SELECT *
            FROM coretoolssystem";

            IEnumerable<CoreToolDto> toolsQueryResult = await connection.QueryAsync<CoreToolDto>(toolsQuery);

            if (toolsQueryResult == null)
                return null;

            return toolsQueryResult;
        }

        public async Task<IEnumerable<AIToolDto>> GetAIToolsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var toolsQuery = @"
            SELECT *
            FROM aitoolssystem";

            IEnumerable<AIToolDto> toolsQueryResult = await connection.QueryAsync<AIToolDto>(toolsQuery);

            if (toolsQueryResult == null)
                return null;

            return toolsQueryResult;
        }

        public async Task<IEnumerable<AppDto>> GetAppsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var toolsQuery = @"
            SELECT *
            FROM appssystem";

            IEnumerable<AppDto> toolsQueryResult = await connection.QueryAsync<AppDto>(toolsQuery);

            if (toolsQueryResult == null)
                return null;

            return toolsQueryResult;
        }


    }
}
