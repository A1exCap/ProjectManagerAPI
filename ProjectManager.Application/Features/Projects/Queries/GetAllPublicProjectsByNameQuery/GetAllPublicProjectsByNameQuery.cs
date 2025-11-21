using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Queries.GetAllProjectsByUserIdQuery;
using ProjectManager.Application.Mappers;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Queries.GetAllPublicProjectsByNameQuery
{
    public record GetAllPublicProjectsByNameQuery(string ProjectName, ProjectQueryParams QueryParams) : IRequest<PagedResult<ProjectDto>>;
}
