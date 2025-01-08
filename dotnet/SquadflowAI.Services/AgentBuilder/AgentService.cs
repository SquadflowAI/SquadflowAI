using Newtonsoft.Json;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
            //var agent = new Agent();
            //var tools = new List<Tool>();

            //agent.LLM = "llm";
            //agent.Role = "role";
            //agent.Tools = tools;

            string baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?
                                    .Parent?.Parent?.Parent?.Parent?.FullName;

            string jsonFilePath = Path.Combine(baseDirectory, "examples" , "football-agent-example-configuration.json");
            string fileContent = File.ReadAllText(jsonFilePath);
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            Agent agent = JsonConvert.DeserializeObject<Agent>(fileContent, settings);

            // Save agent
            await _agentConfigurationRepository.CreateAgentAsync(agent);
        }

        public async Task<Agent> GetAgentByNameAsync(string agentName)
        {
            var result = await _agentConfigurationRepository.GetAgentByNameAsync(agentName);

            return result;
        }

        public async Task RunAgentAsync(string agentName)
        {
            // Get Agent
            Agent agent = await GetAgentByNameAsync(agentName);

            // Generate System Promt
            // Get Tools
            // Call LLM
        }
    }
}
