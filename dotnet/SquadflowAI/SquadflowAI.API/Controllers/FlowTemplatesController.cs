using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Contracts.User;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.Services;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowTemplatesController : ControllerBase
    {
        private readonly IFlowTemplatesRepository _flowTemplatesRepository;

        public FlowTemplatesController(IFlowTemplatesRepository flowTemplatesRepository)
        {
            _flowTemplatesRepository = flowTemplatesRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetFlowTemplates()
        {
            var flowTemplates = await _flowTemplatesRepository.GetFlowTemplatesAsync();
            if (flowTemplates == null)
            {
                return Ok();
            }
            return Ok(flowTemplates);
        }
    }
}
