using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.AgentBuilder
{
    public class AgentService : IAgentService
    {
        private IAgentRepository _agentConfigurationRepository;
        public AgentService(IAgentRepository agentConfigurationRepository) 
        {
            _agentConfigurationRepository = agentConfigurationRepository;
        }

        public async Task CreateAgentAsync()
        {
            // Create Agent
            var agent = new Agent();
            var tools = new List<Tool>();

            agent.LLM = "llm";
            agent.Role = "role";
            agent.Tools = tools;

            // Save agent
            await _agentConfigurationRepository.CreateAgentAsync(agent);
        }

        public async Task<Agent> GetAgentByNameAsync(string name)
        {
            var result = await _agentConfigurationRepository.GetAgentByNameAsync(name);

            return result;
        }

        public async Task RunAgentAsync()
        {
            // Get Agent

            // Run Agent
        }
    }
}
