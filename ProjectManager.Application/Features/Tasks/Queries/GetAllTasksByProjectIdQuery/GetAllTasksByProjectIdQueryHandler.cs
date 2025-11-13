using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Mappers;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Tasks.Queries.GetAllTasksByProjectIdQuery
{
    public class GetAllTasksByProjectIdQueryHandler : IRequestHandler<GetAllTasksByProjectIdQuery, ICollection<ProjectTaskDto>>
    {
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<GetAllTasksByProjectIdQueryHandler> _logger;
        public GetAllTasksByProjectIdQueryHandler(IProjectTaskRepository projectTaskRepository, IProjectRepository projectRepository, 
            ILogger<GetAllTasksByProjectIdQueryHandler> logger)
        {
            _logger = logger;
            _projectTaskRepository = projectTaskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<ICollection<ProjectTaskDto>> Handle(GetAllTasksByProjectIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllTasksByProjectIdQuery for projectId: {ProjectId}", request.projectId);

            var projectExists = await _projectRepository.ExistsAsync(request.projectId);
            if (!projectExists)
            {
                _logger.LogWarning("Project with ID {ProjectId} does not exist", request.projectId);
                throw new NotFoundException($"Project with ID {request.projectId} does not exist.");
            }

            var tasks = await _projectTaskRepository.GetAllTasksByProjectIdAsync(request.projectId);
            _logger.LogInformation("Retrieved {Count} tasks for projectId: {ProjectId}", tasks.Count, request.projectId);

            return tasks.Select(ProjectTaskMapper.ToDto).ToList();
        }
    }
}
