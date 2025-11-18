using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.ProjectDocument;
using ProjectManager.Application.DTOs.ProjectUser;
using ProjectManager.Application.Features.ProjectDocuments.Queries.GetAllDocumentsByProjectIdQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Queries.GetAllUsersByProjectIdQuery
{
    public record GetAllUsersByProjectIdQuery(int ProjectId, string UserId, UsersQueryParams QueryParams) : IRequest<PagedResult<ProjectUserDto>>;
}
