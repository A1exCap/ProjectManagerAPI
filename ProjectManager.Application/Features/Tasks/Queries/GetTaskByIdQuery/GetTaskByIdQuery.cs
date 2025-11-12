using MediatR;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.DTOs.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery
{
    public record class GetTaskByIdQuery(int taskId) : IRequest<ProjectTaskDetailsDto>;
}
