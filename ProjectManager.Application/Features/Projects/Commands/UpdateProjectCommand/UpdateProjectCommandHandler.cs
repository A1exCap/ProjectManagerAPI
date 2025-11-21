using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Tasks.Commands.UpdateTask;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.Projects.Commands.UpdateProjectCommand
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Unit>
    {
        private readonly ILogger<UpdateProjectCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        public UpdateProjectCommandHandler(ILogger<UpdateProjectCommandHandler> logger, IEntityValidationService entityValidationService,
            IAccessService accessService, IProjectRepository projectRepository, IUnitOfWork unitOfWork) 
        {
            _entityValidationService = entityValidationService;
            _projectRepository = projectRepository;
            _accessService = accessService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Unit> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateProjectCommand by projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserIsProjectOwnerAsync(request.ProjectId, request.UserId);

            var project = await _projectRepository.GetByProjectIdAsync(request.ProjectId);

            project.Name = request.Dto.Name;
            project.Description = request.Dto.Description;
            project.EndDate = request.Dto.EndDate;
            project.Status = request.Dto.Status;
            project.Visibility = request.Dto.Visibility;
            project.ClientName = request.Dto.ClientName;
            project.Budget = request.Dto.Budget;
            project.Technologies = request.Dto.Technologies;

            _projectRepository.UpdateProject(project);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Project with ID {ProjectId} updated successfully", request.ProjectId);

            return Unit.Value;
        }
    }
}
