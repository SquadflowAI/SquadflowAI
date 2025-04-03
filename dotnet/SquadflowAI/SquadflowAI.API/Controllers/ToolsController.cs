using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Services.Interfaces;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToolsController : ControllerBase
    {
        private readonly IToolsService _toolsService;

        public ToolsController(IToolsService toolsService)
        {
            _toolsService = toolsService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUIOrchestrations()
        {
            var tools = await _toolsService.GetToolsAsync();
            if (tools == null)
            {
                return NotFound();
            }
            return Ok(tools);
        }
    }
}
