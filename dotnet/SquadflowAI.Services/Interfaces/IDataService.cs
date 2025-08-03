using Microsoft.AspNetCore.Http;
using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Services.Interfaces
{
    public interface IDataService
    {
        Task InsertDocumentFileAsync(Guid projectId, IFormFile file);
        Task<IEnumerable<FilesDocument>> GetFileDocumentsByProjectIdAsync(Guid projectId);
        Task DeleteFileDocumentByIdAsync(Guid id);
    }
}
