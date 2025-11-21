using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Features.ProjectUsers.Commands.DeleteUserFromProjectCommand
{
    public class DeleteUserFromProjectCommandHandler : IRequestHandler<DeleteUserFromProjectCommand, Unit>
    {
        private readonly ILogger<DeleteUserFromProjectCommandHandler> _logger;
        private readonly IAccessService _accessService;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteUserFromProjectCommandHandler(ILogger<DeleteUserFromProjectCommandHandler> logger, IAccessService accessService,
            IEntityValidationService entityValidationService, IProjectUserRepository projectUserRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _entityValidationService = entityValidationService;
            _accessService = accessService;
            _logger = logger;
            _projectUserRepository = projectUserRepository;
        }
        public async Task<Unit> Handle(DeleteUserFromProjectCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling DeleteUserFromProjectCommand by user id: {UserId} and project id: {ProjectId}", request.UserId, request.ProjectId);

            await _entityValidationService.EnsureProjectExistsAsync(request.ProjectId);
            await _entityValidationService.EnsureUserIsProjectMemberAsync(request.ProjectId, request.UserId);
            await _accessService.EnsureUserHasRoleAsync(request.ProjectId, request.CurrentUserId, ["Manager", "Owner"]);

            var projectUser = await _projectUserRepository.GetProjectUserdAsync(request.ProjectId, request.UserId);

            _projectUserRepository.DeleteProjectUser(projectUser);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ProjectUser with id:{ProjectUserId} deleted Succesfully", projectUser.Id);
            return Unit.Value;
        }
    }
}
