using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Commands.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Unit>
    {
        private readonly ILogger<UpdateUserRoleCommandHandler> _logger;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICreateMessageService _messageService;
        private readonly IAccessService _accessService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserRoleCommandHandler(ILogger<UpdateUserRoleCommandHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, IProjectUserRepository projectUserRepository, IUnitOfWork unitOfWork,
            ICreateMessageService messageService)
        {
            _messageService = messageService;
            _projectUserRepository = projectUserRepository;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<Unit> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateUserRoleCommand by userId: {UserId} and projectId: {ProjectId}", request.UserId, request.ProjectId);

            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.CurrentUserId, ["Manager", "Owner"]);
            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureUserIsProjectMemberAsync(request.ProjectId, request.UserId);

            var projectUser = await _projectUserRepository.GetProjectUserdAsync(request.ProjectId, request.UserId);

            var newRole = _entityValidationService.EnsureRoleIsValid(request.NewRole);
            projectUser.Role = newRole;

            _projectUserRepository.UpdateProjectUser(projectUser);
            await _messageService.CreateAsync(request.UserId, NotificationType.ProjectRoleChanged, $"Your role in project ID {request.ProjectId} has been changed to {request.NewRole}.", RelatedEntityType.Project, request.ProjectId);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User role updated successfully, user id: {UserId}, project id: {ProjectId}", request.UserId, request.ProjectId);
            return Unit.Value;
        }
    }
}
