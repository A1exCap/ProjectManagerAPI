using ProjectManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Domain.Interfaces.Repositories
{
    public interface IProjectDocumentRepository
    {
        Task AddDocumentAsync(ProjectDocument document);
        Task<ProjectDocument> GetDocumentByIdAsync(int documentId);
        IQueryable<ProjectDocument> GetAllDocumentsByProjectId(int projectId);
        Task DeleteDocumentByIdAsync(int documentId);
    }
}
