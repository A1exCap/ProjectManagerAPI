using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;

namespace ProjectManager.Application.Features.Projects.Commands.DeleteProjectCommand
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Unit>
    {
        private readonly ILogger<DeleteProjectCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProjectRepository _projectRepository;
        public DeleteProjectCommandHandler(ILogger<DeleteProjectCommandHandler> logger, IEntityValidationService entityValidationService,
            IAccessService accessService, IUnitOfWork unitOfWork, IProjectRepository projectRepository) 
        {
            _logger = logger;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _unitOfWork = unitOfWork;
            _projectRepository = projectRepository;
        }
        public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteProjectCommand by projectId: {ProjectId}", request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserIsProjectOwnerAsync(request.ProjectId, request.UserId);

            await _projectRepository.DeleteProjectAsync(request.ProjectId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Project with ID {ProjectId} deleted successfully", request.ProjectId);

            return Unit.Value;
        }
    }
}
