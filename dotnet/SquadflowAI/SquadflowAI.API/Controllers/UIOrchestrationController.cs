using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.Interfaces;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UIOrchestrationController : ControllerBase
    {
        private readonly IUIOrchestrationService _iUIOrchestrationService;

        public UIOrchestrationController(IUIOrchestrationService iUIOrchestrationService)
        {
            _iUIOrchestrationService = iUIOrchestrationService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUIOrchestration([FromBody] UIOrchestrationDto body)
        {
            await _iUIOrchestrationService.CreateUIOrchestrationAsync(body);
            return Ok();
        }

        [HttpGet("{orchestrationName}")]
        public async Task<IActionResult> GetUIOrchestrationByName(string orchestrationName)
        {
            var orchestration = await _iUIOrchestrationService.GetUIOrchestrationByNameAsync(orchestrationName);
            if (orchestration == null)
            {
                return NotFound();
            }
            return Ok(orchestration);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUIOrchestrations()
        {
            var orchestration = await _iUIOrchestrationService.GetUIOrchestrationsAsync();
            if (orchestration == null)
            {
                return NotFound();
            }
            return Ok(orchestration);
        }
    }
}
