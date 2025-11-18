using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.TaskDocument;

namespace ProjectManager.Application.Features.TaskAttachments.Queries.GetAllAttachmentsByTaskIdQuery
{
    public record GetAllAttachmentsByTaskIdQuery(int ProjectId, string UserId, int TaskId, AttachmentQueryParams QueryParams) : IRequest<PagedResult<TaskAttachmentDto>>;
}
