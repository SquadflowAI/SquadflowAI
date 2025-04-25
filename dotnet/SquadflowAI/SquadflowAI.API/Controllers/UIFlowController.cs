using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Services.Interfaces;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UIFlowController : ControllerBase
    {
        private readonly IUIFlowService _iUIFlowService;

        public UIFlowController(IUIFlowService iUIFlowService)
        {
            _iUIFlowService = iUIFlowService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUIFlow([FromBody] UIFlowDto body)
        {
            await _iUIFlowService.CreateUIFlowAsync(body);
            return Ok();
        }

        [HttpGet("{flowName}")]
        public async Task<IActionResult> GetUIFlowByName(string flowName)
        {
            var uiflow = await _iUIFlowService.GetUIFlowByNameAsync(flowName);
            if (uiflow == null)
            {
                return NotFound();
            }
            return Ok(uiflow);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUIFlows()
        {
            var uiflow = await _iUIFlowService.GetUIFlowsAsync();
            if (uiflow == null)
            {
                return NotFound();
            }
            return Ok(uiflow);
        }
    }
}
