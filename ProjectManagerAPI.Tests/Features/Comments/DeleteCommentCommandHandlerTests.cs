using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Features.Comments.Commands.DeleteCommentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Comments
{
    public class DeleteCommentCommandHandlerTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly IAccessService _accessService;
        private readonly ICommentRepository _commentRepository;

        private readonly DeleteCommentCommandHandler _handler;

        public DeleteCommentCommandHandlerTests()
        {
            _logger = NullLogger<DeleteCommentCommandHandler>.Instance;

            _unitOfWork = A.Fake<IUnitOfWork>();     
            _entityValidationService = A.Fake<IEntityValidationService>();
            _accessService = A.Fake<IAccessService>();
            _commentRepository = A.Fake<ICommentRepository>();

            _handler = new DeleteCommentCommandHandler(_unitOfWork, _entityValidationService, _commentRepository,
                _logger, _accessService);
        }

        [Fact]
        public async Task Handle_ShouldDeleteComment()
        {
            // Arrange
            int projectId = 1;
            int taskId = 1;
            int commentId = 1;
            var userId = "user-123";
            var command = new DeleteCommentCommand(projectId, taskId, userId, commentId);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _entityValidationService.EnsureCommentBelongsToTaskAsync(commentId, taskId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserIsCommentAuthorAsync(userId, commentId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _commentRepository.DeleteCommentByIdAsync(commentId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            result.Should().Be(Unit.Value);
        }
    }
}
