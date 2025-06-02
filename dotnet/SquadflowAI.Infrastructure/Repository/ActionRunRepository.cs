using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class ActionRunRepository : IActionRunRepository
    {
        private readonly DbContext _dbContext;
        public ActionRunRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateActionRunAsync(Guid? agentId, Guid? flowId, string data)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var now = DateTime.Now;

            var actionRunQuery = "INSERT INTO actionRun (agentid, flowid, createddate, data) VALUES (@agentId, @flowId, @now, @data)";
            
            await connection.ExecuteAsync(actionRunQuery, new { agentId, flowId, now, data });
        }

        public async Task CreateActionRunForByteDataAsync(Guid? agentId, Guid? flowId, byte[] bytedata)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var now = DateTime.Now;

            var actionRunQuery = "INSERT INTO actionRun (agentid, flowid, createddate, bytedata) VALUES (@agentId, @flowId, @now, @bytedata)";

            await connection.ExecuteAsync(actionRunQuery, new { agentId, flowId, now, bytedata });
        }

        public async Task<IEnumerable<ActionRun>> GetActionRunsByAgentIdAsync(Guid agentId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var actionRunQuery = @"
            SELECT *
            FROM actionRun a
            WHERE a.agentid = @agentId LIMIT 10";

            var agentResult = await connection.QueryAsync<ActionRun>(actionRunQuery, new { agentId = agentId });

            return agentResult;

        }

        public async Task<IEnumerable<ActionRun>> GetActionRunsByFlowIdAsync(Guid flowId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var actionRunQuery = @"
            SELECT *
            FROM actionRun a
            WHERE a.flowid = @flowId LIMIT 10";

            var agentResult = await connection.QueryAsync<ActionRun>(actionRunQuery, new { flowId = flowId });

            return agentResult;
        }



        public async Task<string> GetActionRunDataByNameAndAgentNameAsyncOBSOLETE(string agent, string actionName)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var actionRunQuery = @"
            SELECT a.data
            FROM actionRun a
            WHERE a.name = @name 
            AND a.agentname = @agentname";

            string agentResult = await connection.QuerySingleOrDefaultAsync<string>(actionRunQuery, new { name = actionName, agentname = agent });

            //var result = JsonConvert.DeserializeObject<Agent>(agentResult);

            return agentResult;
        }

        public async Task<byte[]> GetActionRunByteDataByNameAndAgentNameAsyncOBSOLETE(string agent, string actionName)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var actionRunQuery = @"
            SELECT a.bytedata
            FROM actionRun a
            WHERE a.name = @name 
            AND a.agentname = @agentname
            AND a.bytedata IS NOT NULL";

            var agentResult = await connection.QuerySingleOrDefaultAsync<byte[]>(actionRunQuery, new { name = actionName, agentname = agent });

            //var result = JsonConvert.DeserializeObject<Agent>(agentResult);

            return agentResult;
        }
    }
}
