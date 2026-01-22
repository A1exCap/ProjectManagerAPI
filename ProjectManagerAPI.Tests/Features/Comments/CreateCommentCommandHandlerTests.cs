using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.Comment;
using ProjectManager.Application.Features.Comments.Commands.CreateCommentCommand;
using ProjectManager.Application.Features.Projects.Commands.CreateProjectCommand;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.Validation;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Features.Comments
{
    public class CreateCommentCommandHandlerTests
    {
        private readonly ILogger<CreateCommentCommandHandler> _logger;
        private readonly IEntityValidationService _entityValidationService;
        private readonly ICreateMessageService _messageService;
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessService _accessService;
        private readonly IProjectTaskRepository _projectTaskRepository;

        private readonly CreateCommentCommandHandler _handler;

        public CreateCommentCommandHandlerTests()
        {
            _logger = NullLogger<CreateCommentCommandHandler>.Instance;

            _entityValidationService = A.Fake<IEntityValidationService>();
            _messageService = A.Fake<ICreateMessageService>();
            _commentRepository = A.Fake<ICommentRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _accessService = A.Fake<IAccessService>();
            _projectTaskRepository = A.Fake<IProjectTaskRepository>();

            _handler = new CreateCommentCommandHandler(_logger, _entityValidationService, _commentRepository,
                _unitOfWork, _accessService, _messageService, _projectTaskRepository);
        }

        [Fact]
        public async Task Handle_ShouldCreateComment()
        {
            // Arrange

            int projectId = 1;
            int taskId = 1;
            var userId = "user-123";
            var dto = new CreateCommentDto
            {
                Content = "This is a test comment."
            };

            Comment comment = null;

            var task = new ProjectTask
            {
                Id = taskId,
                Title = "Test Task",
                AssigneeId = "assignee-456"
            };

            A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId))
                .Returns(task);

            A.CallTo(() => _commentRepository.AddCommentAsync(A<Comment>._))
                .Invokes((Comment c) =>
                {
                    c.Id = 10;
                    comment = c; 
                })
                .Returns(Task.CompletedTask);

            var command = new CreateCommentCommand(projectId, taskId, userId, dto);

            // Act
             
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            A.CallTo(() => _entityValidationService.EnsureProjectExistsAsync(projectId))
                .MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _entityValidationService.EnsureTaskBelongsToProjectAsync(taskId, projectId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _accessService.EnsureUserHasAccessAsync(projectId, userId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _projectTaskRepository.GetTaskByIdAsync(taskId)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _commentRepository.AddCommentAsync(A<Comment>._)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _messageService.CreateAsync(task.AssigneeId, NotificationType.NewComment, $"New comment in task: {task.Title}", RelatedEntityType.Comment, task.Id)).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None)).MustHaveHappenedOnceExactly());

            comment.Should().NotBeNull();
            comment.Content.Should().Be(dto.Content);
            comment.TaskId.Should().Be(taskId);
            comment.AuthorId.Should().Be(userId);   

            result.Should().Be(10);
        }
    }
}
