using MediatR;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Queries.GetProjectDetailsByIdQuery
{
    public record GetProjectDetailsByIdQuery(int ProjectId, string UserId) : IRequest<ProjectDetailsDto>;
}
