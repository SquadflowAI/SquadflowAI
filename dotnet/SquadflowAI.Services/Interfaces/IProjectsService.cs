using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IProjectsService
    {
        Task CreateProjectAsync(Project flow);

        Task<Project> GetProjectByNameAsync(string name);

        Task<IEnumerable<Project>> GetProjectsAsync();

        Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId);
    }
}
