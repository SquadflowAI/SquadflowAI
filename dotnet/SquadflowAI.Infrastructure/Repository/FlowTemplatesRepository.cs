using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Contracts;
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
    public class FlowTemplatesRepository : IFlowTemplatesRepository
    {
        private readonly DbContext _dbContext;
        public FlowTemplatesRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FlowTemplate>> GetFlowTemplatesAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var flowTemplatesQuery = @"
            SELECT *
            FROM flowtemplates a";

            IEnumerable<FlowTemplate> flowTemplatesQueryResult = await connection.QueryAsync<FlowTemplate>(flowTemplatesQuery);

            foreach (var item in flowTemplatesQueryResult)
            {
                item.DataConverted = JsonConvert.DeserializeObject<UIFlow>(item.Data);
            }
            if (flowTemplatesQueryResult == null)
                return null;

            return flowTemplatesQueryResult;
        }
    }
}
