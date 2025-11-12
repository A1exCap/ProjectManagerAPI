using MediatR;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public record class GetAllTasksByProjectIdQuery(int projectId) : IRequest<ICollection<ProjectTaskDto>>;
}
