using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.TaskDocument;
using ProjectManager.Application.Features.TaskAttachments.Queries.GetAllAttachmentsByTaskIdQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery
{
    public record GetAllDocumentsByProjectIdQuery(int ProjectId, string UserId, DocumentQueryParams QueryParams) : IRequest<PagedResult<ProjectDocumentDto>>;
}
