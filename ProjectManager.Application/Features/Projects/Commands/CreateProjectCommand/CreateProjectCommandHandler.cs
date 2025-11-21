using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Commands.CreateTask;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Commands.CreateProjectCommand
{
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
    {
        private readonly ILogger<CreateProjectCommandHandler> _logger;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProjectCommandHandler(ILogger<CreateProjectCommandHandler> logger, IProjectUserRepository projectUserRepository,
            IProjectRepository projectRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _projectUserRepository = projectUserRepository;
            _projectRepository = projectRepository;
        }

        public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateProjectCommandHandler with projectName: {ProjectName}", request.dto.Name);

            Project project = new Project
            {
                Name = request.dto.Name,
                Description = request.dto.Description,
                EndDate = request.dto.EndDate,
                Status = request.dto.Status,
                Visibility = request.dto.Visibility,
                ClientName = request.dto.ClientName,
                Budget = request.dto.Budget,
                Technologies = request.dto.Technologies
            };

            await _projectRepository.AddProjectAsync(project);
            var createdProject = await _projectRepository.GetByProjectIdAsync(project.Id);

            ProjectUser owner = new ProjectUser
            {
                ProjectId = createdProject.Id,
                UserId = request.UserId,
                Role = ProjectUserRole.Owner
            };

            await _projectUserRepository.AddProjectUserAsync(owner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created project with ID {ProjectId}", createdProject.Id);
            return createdProject.Id;
        }
    }
}
