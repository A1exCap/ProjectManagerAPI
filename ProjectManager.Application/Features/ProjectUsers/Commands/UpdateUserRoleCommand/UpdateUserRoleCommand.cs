using MediatR;
using ProjectManager.Application.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Commands.UpdateUserRoleCommand
{
    public record UpdateUserRoleCommand(int ProjectId, string CurrentUserId, string UserId, string NewRole) : IRequest<Unit>;
}
