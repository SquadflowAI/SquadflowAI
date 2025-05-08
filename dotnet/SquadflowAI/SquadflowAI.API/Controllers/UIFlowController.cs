using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Services.Interfaces;
using SquadflowAI.Services.Services;

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

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUIFlow([FromBody] UIFlowDto body)
        {
            await _iUIFlowService.UpdateUIFlowAsync(body);
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

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetUIFlowById(string id)
        {
            var flowId = new Guid(id);
            var uiflow = await _iUIFlowService.GetUIFlowByIdAsync(flowId);
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

        [HttpGet("project-id/{projectId}")]
        public async Task<IActionResult> GetProjectsByUserId(string projectId)
        {
            var userGuidId = new Guid(projectId);
            var uiflows = await _iUIFlowService.GetUIFlowsByProjectIdAsync(userGuidId);

            if (uiflows == null)
            {
                return NotFound();
            }

            return Ok(uiflows);
        }
    }
}
