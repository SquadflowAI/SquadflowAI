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

        public async Task SaveActionAsync(string agentName, string actionName, string data)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var now = DateTime.Now;

            var actionRunQuery = "INSERT INTO actionRun (agentname, name, date, data) VALUES (@agentName, @actionName, @now, @data)";
            
            await connection.ExecuteAsync(actionRunQuery, new { agentName, actionName, now, data });
        }
    }
}
