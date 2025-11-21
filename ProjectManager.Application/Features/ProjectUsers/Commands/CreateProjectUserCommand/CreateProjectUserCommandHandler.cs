using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Comments.Commands.CreateCommentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Commands.CreateProjectUserCommand
{
    public class CreateProjectUserCommandHandler : IRequestHandler<CreateProjectUserCommand, int>
    {
        private readonly ILogger<CreateCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateProjectUserCommandHandler(ILogger<CreateCommentCommandHandler> logger, IEntityValidationService entityValidationService, 
            IAccessService accessService, IProjectUserRepository projectUserRepository, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _projectUserRepository = projectUserRepository;
            _accessService = accessService;
            _entityValidationService = entityValidationService;
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(CreateProjectUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling CreateProjectUserCommand with userId: {UserId}, projectId: {ProjectId}", request.dto.UserToAddId, request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.UserId, ["Manager"]);

            var userRole = _entityValidationService.EnsureRoleIsValid(request.dto.UserRole);

            var projectUser = new ProjectUser
            {
                ProjectId = request.ProjectId,
                UserId = request.dto.UserToAddId,
                Role = userRole
            };

            await _projectUserRepository.AddProjectUserAsync(projectUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created projectUser with ID: {ProjectUserId}", projectUser.Id);
            return projectUser.Id;
        }
    }
}
