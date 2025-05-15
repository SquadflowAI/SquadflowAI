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
        public async Task<IActionResult> GetUITools()
        {
            var tools = await _toolsService.GetToolsAsync();
            if (tools == null)
            {
                return NotFound();
            }
            return Ok(tools);
        }

        [HttpGet("all/core")]
        public async Task<IActionResult> GetUICoreTools()
        {
            var tools = await _toolsService.GetCoreToolsAsync();
            if (tools == null)
            {
                return NotFound();
            }
            return Ok(tools);
        }

        [HttpGet("all/ai")]
        public async Task<IActionResult> GetUIAITools()
        {
            var tools = await _toolsService.GetAIToolsAsync();
            if (tools == null)
            {
                return NotFound();
            }
            return Ok(tools);
        }


        [HttpGet("all/apps")]
        public async Task<IActionResult> GetUIApps()
        {
            var tools = await _toolsService.GetAppsAsync();
            if (tools == null)
            {
                return NotFound();
            }
            return Ok(tools);
        }
    }
}
