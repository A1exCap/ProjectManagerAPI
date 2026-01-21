using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.Features.Comments.Commands.UpdateCommentCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Comments
{
    public class UpdateCommentCommandHandlerTests
    {
        private readonly ILogger<UpdateCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;

        private readonly UpdateCommentCommandHandler _handler;

        public UpdateCommentCommandHandlerTests()
        {
            _logger = NullLogger<UpdateCommentCommandHandler>.Instance;
            _entityValidationService = A.Fake<IEntityValidationService>();
            _commentRepository = A.Fake<ICommentRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _accessService = A.Fake<IAccessService>();

            _handler = new UpdateCommentCommandHandler(_logger, _entityValidationService, _commentRepository,
                _unitOfWork, _accessService);
        }

        [Fact]
        public async Task Handle_ShouldUpdateComment()
        {
            // Arrange

            int projectId = 1;
            int taskId = 1;
            int commentId = 1;
            var userId = "user-123";

            var dto = new UpdateCommentDto
            {
                Content = "Updated comment content."
            };

            var command = new UpdateCommentCommand(projectId, taskId, commentId, userId, dto);

            var existingComment = new Comment
            {
                Id = commentId,
                TaskId = taskId,
                AuthorId = userId,
                Content = "Old content",
                Edited = false
            };

            A.CallTo(() => _commentRepository.GetByIdAsync(commentId))
            .Returns(existingComment);

            // Act

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert

            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
            .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _entityValidationService.EnsureCommentBelongsToTaskAsync(commentId, taskId)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _accessService.EnsureUserIsCommentAuthorAsync(userId, commentId)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _commentRepository.GetByIdAsync(commentId)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _commentRepository.UpdateComment(existingComment)).MustHaveHappenedOnceExactly())
            .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            existingComment.Content.Should().Be(dto.Content);
            existingComment.Edited.Should().BeTrue();

            result.Should().Be(Unit.Value);
        }
    }
}
