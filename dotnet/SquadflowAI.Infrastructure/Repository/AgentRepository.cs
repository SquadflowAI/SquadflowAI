using Dapper;
using Newtonsoft.Json;
using Npgsql;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System.Collections;

//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class AgentRepository : IAgentRepository
    {
        private readonly DbContext _dbContext;
        public AgentRepository(DbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task CreateAgentAsync(Agent agent)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();
            
            var data = JsonConvert.SerializeObject(agent);
            var agentQuery = "INSERT INTO agents (name, data) VALUES (@name, @data::jsonb)";
            await connection.ExecuteAsync(agentQuery, new { agent.Name, data });
        }

        public async Task<Agent> GetAgentByNameAsync(string name)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var agentQuery = @"
            SELECT a.data
            FROM agents a
            WHERE a.name = @name";

            string agentResult = await connection.QuerySingleOrDefaultAsync<string>(agentQuery, new { name = name });

            var result = JsonConvert.DeserializeObject<Agent>(agentResult);

            return result;
        }

    }

 

}
