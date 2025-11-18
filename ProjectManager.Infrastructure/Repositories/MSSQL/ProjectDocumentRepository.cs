using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Repositories.MSSQL
{
    public class ProjectDocumentRepository : IProjectDocumentRepository
    {
        private readonly ApplicationDbContext _context;
        public ProjectDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddDocumentAsync(ProjectDocument document)
        {
            await _context.ProjectDocuments.AddAsync(document);
        }

        public async Task DeleteDocumentByIdAsync(int documentId)
        {
            var document = await GetDocumentByIdAsync(documentId);
            _context.ProjectDocuments.Remove(document);
        }

        public IQueryable<ProjectDocument> GetAllDocumentsByProjectId(int projectId)
        {
            return _context.ProjectDocuments
               .Where(t => t.ProjectId == projectId)
               .AsQueryable();
        }

        public async Task<ProjectDocument?> GetDocumentByIdAsync(int documentId)
        {
            return await _context.ProjectDocuments.Include(d=>d.Project).Include(d=>d.UploadedBy).FirstOrDefaultAsync(d=>d.Id==documentId);
        }
    }
}
