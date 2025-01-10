using Dapper;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            using var transaction = connection.BeginTransaction();

            try
            {
                var configQuery = "INSERT INTO Configurations (EmailRecipient, ReportSchedule) VALUES (@EmailRecipient, @ReportSchedule) RETURNING Id";
                var configurationId = await connection.ExecuteScalarAsync<int>(configQuery, new { agent.Configuration.EmailRecipient, agent.Configuration.ReportSchedule });
 
                var agentQuery = "INSERT INTO Agents (Name, Mission, ConfigurationId) VALUES (@Name, @Mission, @ConfigurationId) RETURNING Id";
                var agentId = await connection.ExecuteScalarAsync<int>(agentQuery, new { agent.Name, agent.Mission, configurationId });

                foreach (var capability in agent.Capabilities)
                {
                    var capabilityQuery = "INSERT INTO Capabilities (AgentId, Task, UseCase) VALUES (@AgentId, @Task, @UseCase)";
                    await connection.ExecuteAsync(capabilityQuery, new { AgentId = agentId, capability.Task, capability.UseCase });
                }

                foreach (var action in agent.Actions)
                {
                    var actionQuery = "INSERT INTO Actions (AgentId, Name, Description, ActionToExecute) VALUES (@AgentId, @Name, @Description, @ActionToExecute) RETURNING Id";
                    var actionId = await connection.ExecuteScalarAsync<int>(actionQuery, new { AgentId = agentId, action.Name, action.Description, action.ActionToExecute });

                    //foreach (var input in action.Inputs)
                    //{
                    //    var actionInputQuery = "INSERT INTO ActionInput (ActionId, Input) VALUES (@ActionId, @Input)";
                    //    await connection.ExecuteAsync(actionInputQuery, new { ActionId = actionId, Input = input });
                    //}

                    //foreach (var output in action.Outputs)
                    //{
                    //    var actionOutputQuery = "INSERT INTO ActionOutput (ActionId, Output) VALUES (@ActionId, @Output)";
                    //    await connection.ExecuteAsync(actionOutputQuery, new { ActionId = actionId, Output = output });
                    //}

                    //foreach (var trigger in action.Triggers)
                    //{
                    //    var actionTriggerQuery = "INSERT INTO ActionTrigger (ActionId, Trigger) VALUES (@ActionId, @Trigger)";
                    //    await connection.ExecuteAsync(actionTriggerQuery, new { ActionId = actionId, Trigger = trigger });
                    //}

                    foreach (var tool in action.Tools)
                    {
                        var toolQuery = "INSERT INTO Tools (Name, Description) VALUES (@Name, @Description) RETURNING Id";
                        var toolId = await connection.ExecuteScalarAsync<int>(toolQuery, new { tool.Name, tool.Description });

                        var actionToolQuery = "INSERT INTO actiontools (ActionId, ToolId) VALUES (@ActionId, @ToolId)";
                        await connection.ExecuteAsync(actionToolQuery, new { ActionId = actionId, ToolId = toolId });
                    }
                }

                // Commit all inserts
                transaction.Commit();
            }
            catch
            {
                // Rollback in case of an error
                transaction.Rollback();
                throw;
            }
        }

 


 

        public async Task<Agent> GetAgentByNameAsync(string agentName)
        {
            return null;
        }
    }


}
