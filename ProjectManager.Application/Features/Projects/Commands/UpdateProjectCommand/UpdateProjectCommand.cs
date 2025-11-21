using MediatR;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.DTOs.ProjectTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Commands.UpdateProjectCommand
{
    public record UpdateProjectCommand(int ProjectId, string UserId, ProjectUpdateDto Dto) : IRequest<Unit>;
}
