using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.User;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.Services;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationsController : ControllerBase
    {
        private readonly IIntegrationsService _integrationsService;

        public IntegrationsController(IIntegrationsService integrationsService)
        {
            _integrationsService = integrationsService;
        }

        [HttpPost("create-integration")]
        public async Task<IActionResult> CreateIntegration([FromBody] IntegrationsDto body)
        {
            await _integrationsService.CreateIntegrationAsync(body);
            return Ok();
        }

        [HttpGet("user-id/{userId}")]
        public async Task<IActionResult> GetIntegrationsByUserId(Guid userId)
        {
            var integration = await _integrationsService.GetIntegrationsByUserIdAsync(userId);
            if (integration == null)
            {
                return NotFound();
            }
            return Ok(integration);
        }
    }
}
