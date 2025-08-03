using Microsoft.AspNetCore.Http;
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
    public class DataService : IDataService
    {
        private readonly IFileDocumentsRepository _fileDocumentsRepository;
        public DataService(IFileDocumentsRepository fileDocumentsRepository) 
        {
            _fileDocumentsRepository = fileDocumentsRepository;
        }

        public async Task InsertDocumentFileAsync(Guid projectId, IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var document = new FilesDocument
            {
                Name = file.FileName,
                ProjectId = projectId,
                Content = memoryStream.ToArray(),
                ContentType = file.ContentType
            };

            var fileId = await _fileDocumentsRepository.CreateFileDocumentAsync(document);
        }

        public async Task<IEnumerable<FilesDocument>> GetFileDocumentsByProjectIdAsync(Guid projectId)
        {
            var projects = await _fileDocumentsRepository.GetFileDocumentsByProjectIdAsync(projectId);

            return projects;
        }

        public async Task DeleteFileDocumentByIdAsync(Guid id) 
        {
            await _fileDocumentsRepository.DeleteFileDocumentByIdAsync(id);
        }
    }
}
