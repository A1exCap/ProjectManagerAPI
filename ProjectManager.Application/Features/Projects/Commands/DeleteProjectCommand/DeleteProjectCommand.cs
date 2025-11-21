using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Commands.DeleteProjectCommand
{
    public record DeleteProjectCommand(int ProjectId, string UserId) : IRequest<Unit>;
}
