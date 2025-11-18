using MediatR;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.TaskAttachment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectDocuments.Queries.DownloadDocumentByIdQuery
{
    public record DownloadDocumentByIdQuery(int ProjectId, int DocumentId, string UserId) : IRequest<DownloadDocumentDto>;
}
