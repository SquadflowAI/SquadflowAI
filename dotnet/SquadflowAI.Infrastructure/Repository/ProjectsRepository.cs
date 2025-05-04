using Dapper;
using Newtonsoft.Json;
using SquadflowAI.Contracts.Dtos;
using SquadflowAI.Domain;
using SquadflowAI.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Repository
{
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly DbContext _dbContext;
        public ProjectsRepository(DbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task CreateProjectAsync(Project project)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var projectsQuery = @"INSERT INTO projects 
            (userId, name, createdDate, updatedDate) 
            VALUES (@userId, @name, @createdDate, @updatedDate)";
            await connection.ExecuteAsync(projectsQuery, new {project.UserId,
                project.Name, project.CreatedDate, project.UpdatedDate });
        }

        public async Task<Project> GetProjectByNameAsync(string inputName)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var projectQuery = @"
            SELECT *
            FROM projects a
            WHERE a.name = @inputName";

            string projectsQueryResult = await connection.QuerySingleOrDefaultAsync<string>(projectQuery, new { inputName = inputName });

            if (projectsQueryResult == null)
                return null;

            var result = JsonConvert.DeserializeObject<Project>(projectsQueryResult);

            return result;
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT *
            FROM projects a";

            IEnumerable<Project> projectsQueryResult = await connection.QueryAsync<Project>(uiflowQuery);

            if (projectsQueryResult == null)
                return null;

            return projectsQueryResult.ToList();
        }

        public async Task<IEnumerable<Project>> GetProjectsByUserIdAsync(Guid userId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowQuery = @"
            SELECT *
            FROM projects a WHERE a.userid = @userId";

            IEnumerable<Project> projectsQueryResult = await connection.QueryAsync<Project>(uiflowQuery, new { userId = userId});

            if (projectsQueryResult == null)
                return null;

            return projectsQueryResult.ToList();
        }
    }
}
