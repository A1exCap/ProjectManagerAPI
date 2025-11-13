using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.DTOs.ProjectTask;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Exceptions;
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
        private readonly ILogger<GetTaskByIdQueryHandler> _logger;
        private readonly IProjectTaskRepository _projectTaskRepository;
        public GetTaskByIdQueryHandler(IProjectTaskRepository projectTaskRepository, ILogger<GetTaskByIdQueryHandler> logger)
        {
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
        }
        public async Task<ProjectTaskDetailsDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTaskByIdQuery by tsak Id: {taskId}", request.taskId);
            var task = await _projectTaskRepository.GetTaskByIdAsync(request.taskId);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} does not exists", request.taskId);
                throw new NotFoundException($"Task with ID {request.taskId} does not exist.");
            }
            _logger.LogInformation("Retrieved task by task id: {taskId}", request.taskId);

            return ProjectTaskMapper.ToDetailDto(task);
        }
    }
}
