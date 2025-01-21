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

        public async Task SaveActionRunAsync(string agentName, string actionName, string data)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var now = DateTime.Now;

            var actionRunQuery = "INSERT INTO actionRun (agentname, name, date, data) VALUES (@agentName, @actionName, @now, @data)";
            
            await connection.ExecuteAsync(actionRunQuery, new { agentName, actionName, now, data });
        }

        public async Task<string> GetActionRunByNameAndAgentNameAsync(string agent, string actionName)
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
    }
}
