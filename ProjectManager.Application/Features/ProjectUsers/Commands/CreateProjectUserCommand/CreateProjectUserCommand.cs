using MediatR;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.ProjectUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Commands.CreateProjectUserCommand
{
    public record CreateProjectUserCommand(int ProjectId, string UserId, AddUserToProjectDto dto) : IRequest<int>;
}
