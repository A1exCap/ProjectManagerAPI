using MediatR;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.DTOs.Task;
using ProjectManager.Application.Mappers;
using ProjectManager.Domain.Interfaces.Repositories;
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
        public GetAllTasksByProjectIdQueryHandler(IProjectTaskRepository projectTaskRepository, IProjectRepository projectRepository)
        {
            _projectTaskRepository = projectTaskRepository;
            _projectRepository = projectRepository;
        }

        public async Task<ICollection<ProjectTaskDto>> Handle(GetAllTasksByProjectIdQuery request, CancellationToken cancellationToken)
        {
            var projectExists = await _projectRepository.ExistsAsync(request.projectId);

            if (!projectExists)
                throw new Exception($"Project with ID {request.projectId} does not exist.");

            var tasks = await _projectTaskRepository.GetAllTasksByProjectIdAsync(request.projectId);

            return tasks.Select(ProjectTaskMapper.ToDto).ToList();
        }
    }
}
