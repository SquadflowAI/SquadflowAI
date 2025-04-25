using Newtonsoft.Json;
using SquadflowAI.Contracts.Tools;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Infrastructure.Repository;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Agent
{
    public class AgentService : IAgentService
    {
        private IAgentRepository _agentRepository;
        public AgentService(IAgentRepository agentConfigurationRepository) 
        {
            _agentRepository = agentConfigurationRepository;
        }

        public async Task CreateAgentAsync()
        {
            // Create Agent
            //var agent = new Agent();
            //var tools = new List<Tool>();

            //agent.LLM = "llm";
            //agent.Role = "role";
            //agent.Tools = tools;

            string? baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.Parent?.Parent?.FullName;

            string jsonFilePath = Path.Combine(baseDirectory, "examples" , "football-agent-example-configuration - Copy.json");
            string fileContent = File.ReadAllText(jsonFilePath);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            Domain.Agent agent = JsonConvert.DeserializeObject<Domain.Agent>(fileContent, settings);

            // Save agent
            await _agentRepository.CreateAgentAsync(agent);
        }

        public async Task<Domain.Agent> GetAgentByNameAsync(string agentName)
        {
            var result = await _agentRepository.GetAgentByNameAsync(agentName);

            return result;
        }

        public async Task RunAgentAsync(string agentName)
        {
            // Get Agent
            Domain.Agent agent = await GetAgentByNameAsync(agentName);

            // Generate System Promt
            // Get Tools
            // Call LLM
        }

        public async Task<IEnumerable<Domain.Agent>> GetAgentAsync()
        {
            var result = await _agentRepository.GetAgentAsync();

            return result;
        }
    }
}
