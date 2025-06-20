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
    public class FileDocumentsRepository : IFileDocumentsRepository
    {
        private readonly DbContext _dbContext;
        public FileDocumentsRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateFileDocumentAsync(FilesDocuments file)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            const string sql = @"
                    INSERT INTO filesdocuments (id, name, content, content_type)
                    VALUES (@Id, @Name, @Content, @ContentType)
                    RETURNING id;";

            // Ensure Id is set before insert
            file.Id = Guid.NewGuid();

            var newId = await connection.ExecuteScalarAsync<Guid>(sql, file);
            return newId;
        }

        public async Task<FilesDocuments?> GetFileDocumentByIdAsync(Guid id)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            const string sql = @"
                SELECT id, name, content, content_type
                FROM filesdocuments
                WHERE id = @Id";

            return await connection.QuerySingleOrDefaultAsync<FilesDocuments>(sql, new { Id = id });
        }

        public async Task DeleteFileDocumentByIdAsync(Guid id)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();

            var uiflowSql = @"
            DELETE
            FROM filesdocuments a
            WHERE a.id = @id";

            await connection.ExecuteAsync(uiflowSql, new { id = id });
        }



    }
}
