using MediatR;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Mappers;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Queries.GetTaskByIdQuery
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, ProjectTaskDetailsDto>
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        public GetTaskByIdQueryHandler(IProjectTaskRepository projectTaskRepository)
        {
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<ProjectTaskDetailsDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            if(!await _projectTaskRepository.ExistsAsync(request.taskId))
                throw new Exception($"Task with ID {request.taskId} does not exist.");

            var task = await _projectTaskRepository.GetTaskByIdAsync(request.taskId);
            return ProjectTaskMapper.ToDetailDto(task);
        }
    }
}
