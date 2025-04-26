using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using SquadflowAI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsRepository _projectsRepository;
        public ProjectsService(IProjectsRepository projectsRepository)
        {
            _projectsRepository = projectsRepository;
        }

        public async Task CreateProjectAsync(Project project)
        {
            project.CreatedDate = DateTime.Now;
            project.UpdatedDate = DateTime.Now; 

            await _projectsRepository.CreateProjectAsync(project);
        }

        public async Task<Project> GetProjectByNameAsync(string name)
        {
            var result = await _projectsRepository.GetProjectByNameAsync(name);

            return result;
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            var result = await _projectsRepository.GetProjectsAsync();

            return result;
        }
    }
}
