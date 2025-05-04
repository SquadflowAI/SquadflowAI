using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Services.Interfaces;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsService _projectService;

        public ProjectsController(IProjectsService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProject([FromBody] Project body)
        {
            await _projectService.CreateProjectAsync(body);
            return Ok();
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetProjectByName(string name)
        {
            var project = await _projectService.GetProjectByNameAsync(name);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _projectService.GetProjectsAsync();
            if (projects == null)
            {
                return NotFound();
            }
            return Ok(projects);
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetProjectsByUserId(string userId)
        {
            var userGuidId = new Guid(userId);
            var projects = await _projectService.GetProjectsByUserIdAsync(userGuidId);
            if (projects == null)
            {
                return NotFound();
            }
            return Ok(projects);
        }
    }
}
