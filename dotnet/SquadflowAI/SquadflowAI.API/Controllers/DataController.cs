using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SquadflowAI.Contracts.Data;
using SquadflowAI.Services.Interfaces;

namespace SquadflowAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IDataService _dataService;
        public DataController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] DataFileUploadDto payload)
        {
            await _dataService.InsertDocumentFileAsync(payload.ProjectId, payload.File);

            return Ok();
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetData(Guid projectId)
        {
            var result = await _dataService.GetFileDocumentsByProjectIdAsync(projectId);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileData(Guid id)
        {
            await _dataService.DeleteFileDocumentByIdAsync(id);

            return Ok();
        }
    }
}
