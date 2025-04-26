using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IProjectsRepository
    {
        Task CreateProjectAsync(Project flow);
        Task<Project> GetProjectByNameAsync(string name);
        Task<IEnumerable<Project>> GetProjectsAsync();
    }
}
