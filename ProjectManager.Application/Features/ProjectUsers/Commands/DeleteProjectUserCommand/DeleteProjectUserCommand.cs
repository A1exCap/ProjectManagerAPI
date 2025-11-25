using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Commands.DeleteUserFromProjectCommand
{
    public record DeleteProjectUserCommand(int ProjectId, string UserId, string CurrentUserId) : IRequest<Unit>;
}
