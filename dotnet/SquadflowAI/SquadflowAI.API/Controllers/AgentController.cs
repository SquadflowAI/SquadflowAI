using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.Services;

namespace SquadflowAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agentService;

        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAgent()
        {
            await _agentService.CreateAgentAsync();
            return Ok();
        }

        [HttpGet("{agentName}")]
        public async Task<IActionResult> GetAgentByName(string agentName)
        {
            var agent = await _agentService.GetAgentByNameAsync(agentName);
            if (agent == null)
            {
                return NotFound();
            }
            return Ok(agent);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAgents()
        {
            var tools = await _agentService.GetAgentAsync();
            if (tools == null)
            {
                return NotFound();
            }
            return Ok(tools);
        }

        //[HttpPost("run/{agentName}")]
        //public async Task<IActionResult> RunAgent(string agentName)
        //{
        //    await _agentService.RunAgentAsync(agentName);
        //    return Ok();
        //}
    }
}
