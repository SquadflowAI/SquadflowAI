using SquadflowAI.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquadflowAI.Infrastructure.Interfaces
{
    public interface IFileDocumentsRepository
    {
        Task<Guid> CreateFileDocumentAsync(FilesDocuments file);
        Task<FilesDocuments?> GetFileDocumentByIdAsync(Guid id);
        Task DeleteFileDocumentByIdAsync(Guid id);
    }
}
