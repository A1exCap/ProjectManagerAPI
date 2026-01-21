using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Exceptions;
using ProjectManager.Application.Services;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using Xunit;

public class EntityValidationServiceTests
{
    private readonly IProjectRepository _projectRepo;
    private readonly IProjectTaskRepository _taskRepo;
    private readonly ICommentRepository _commentRepo;
    private readonly ITaskAttachmentRepository _attachmentRepo;
    private readonly IProjectDocumentRepository _documentRepo;
    private readonly EntityValidationService _service;

    public EntityValidationServiceTests()
    {
        _projectRepo = A.Fake<IProjectRepository>();
        _taskRepo = A.Fake<IProjectTaskRepository>();
        _commentRepo = A.Fake<ICommentRepository>();
        _attachmentRepo = A.Fake<ITaskAttachmentRepository>();
        _documentRepo = A.Fake<IProjectDocumentRepository>();
        var logger = NullLogger<EntityValidationService>.Instance;

        _service = new EntityValidationService(_projectRepo, _taskRepo, logger, _commentRepo, _attachmentRepo, _documentRepo);
    }

    // ==========================================
    // 1. EnsureDocumentBelongsToProjectAsync (3 tests)
    // ==========================================

    [Fact]
    public async Task EnsureDocumentBelongsToProject_ShouldSucceed_WhenValid()
    {
        // Arrange
        var doc = new ProjectDocument { Id = 1, ProjectId = 100 };
        A.CallTo(() => _documentRepo.GetDocumentByIdAsync(1)).Returns(doc);

        // Act & Assert
        await _service.EnsureDocumentBelongsToProjectAsync(1, 100);
    }

    [Fact]
    public async Task EnsureDocumentBelongsToProject_ShouldThrowNotFound_WhenMissing()
    {
        // Arrange
        A.CallTo(() => _documentRepo.GetDocumentByIdAsync(1)).Returns((ProjectDocument?)null);

        // Act
        Func<Task> act = async () => await _service.EnsureDocumentBelongsToProjectAsync(1, 100);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task EnsureDocumentBelongsToProject_ShouldThrowForbidden_WhenProjectMismatch()
    {
        // Arrange
        var doc = new ProjectDocument { Id = 1, ProjectId = 999 }; // Інший проект
        A.CallTo(() => _documentRepo.GetDocumentByIdAsync(1)).Returns(doc);

        // Act
        Func<Task> act = async () => await _service.EnsureDocumentBelongsToProjectAsync(1, 100);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    // ==========================================
    // 2. EnsureAttachmentBelongsToTaskAsync (3 tests)
    // ==========================================

    [Fact]
    public async Task EnsureAttachmentBelongsToTask_ShouldSucceed_WhenValid()
    {
        var att = new TaskAttachment { Id = 1, ProjectTaskId = 50 };
        A.CallTo(() => _attachmentRepo.GetAttachmentByIdAsync(1)).Returns(att);

        await _service.EnsureAttachmentBelongsToTaskAsync(1, 50);
    }

    [Fact]
    public async Task EnsureAttachmentBelongsToTask_ShouldThrowNotFound_WhenMissing()
    {
        A.CallTo(() => _attachmentRepo.GetAttachmentByIdAsync(1)).Returns((TaskAttachment?)null);

        Func<Task> act = async () => await _service.EnsureAttachmentBelongsToTaskAsync(1, 50);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task EnsureAttachmentBelongsToTask_ShouldThrowForbidden_WhenTaskMismatch()
    {
        var att = new TaskAttachment { Id = 1, ProjectTaskId = 999 };
        A.CallTo(() => _attachmentRepo.GetAttachmentByIdAsync(1)).Returns(att);

        Func<Task> act = async () => await _service.EnsureAttachmentBelongsToTaskAsync(1, 50);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    // ==========================================
    // 3. EnsureCommentBelongsToTaskAsync (3 tests)
    // ==========================================

    [Fact]
    public async Task EnsureCommentBelongsToTask_ShouldSucceed_WhenValid()
    {
        var comment = new Comment { Id = 1, TaskId = 50 };
        A.CallTo(() => _commentRepo.GetByIdAsync(1)).Returns(comment);

        await _service.EnsureCommentBelongsToTaskAsync(1, 50);
    }

    [Fact]
    public async Task EnsureCommentBelongsToTask_ShouldThrowNotFound_WhenMissing()
    {
        A.CallTo(() => _commentRepo.GetByIdAsync(1)).Returns((Comment?)null);

        Func<Task> act = async () => await _service.EnsureCommentBelongsToTaskAsync(1, 50);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task EnsureCommentBelongsToTask_ShouldThrowForbidden_WhenTaskMismatch()
    {
        var comment = new Comment { Id = 1, TaskId = 999 };
        A.CallTo(() => _commentRepo.GetByIdAsync(1)).Returns(comment);

        Func<Task> act = async () => await _service.EnsureCommentBelongsToTaskAsync(1, 50);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    // ==========================================
    // 4. EnsureProjectExistsAsync (2 tests)
    // ==========================================

    [Fact]
    public async Task EnsureProjectExists_ShouldSucceed_WhenExists()
    {
        A.CallTo(() => _projectRepo.ExistsAsync(100)).Returns(true);
        await _service.EnsureProjectExistsAsync(100);
    }

    [Fact]
    public async Task EnsureProjectExists_ShouldThrowNotFound_WhenNotExists()
    {
        A.CallTo(() => _projectRepo.ExistsAsync(100)).Returns(false);

        Func<Task> act = async () => await _service.EnsureProjectExistsAsync(100);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // ==========================================
    // 5. EnsureTaskBelongsToProjectAsync (3 tests)
    // ==========================================

    [Fact]
    public async Task EnsureTaskBelongsToProject_ShouldSucceed_WhenValid()
    {
        var task = new ProjectTask { Id = 50, ProjectId = 100 };
        A.CallTo(() => _taskRepo.GetTaskByIdAsync(50)).Returns(task);

        await _service.EnsureTaskBelongsToProjectAsync(50, 100);
    }

    [Fact]
    public async Task EnsureTaskBelongsToProject_ShouldThrowNotFound_WhenMissing()
    {
        A.CallTo(() => _taskRepo.GetTaskByIdAsync(50)).Returns((ProjectTask?)null);

        Func<Task> act = async () => await _service.EnsureTaskBelongsToProjectAsync(50, 100);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task EnsureTaskBelongsToProject_ShouldThrowForbidden_WhenProjectMismatch()
    {
        var task = new ProjectTask { Id = 50, ProjectId = 999 };
        A.CallTo(() => _taskRepo.GetTaskByIdAsync(50)).Returns(task);

        Func<Task> act = async () => await _service.EnsureTaskBelongsToProjectAsync(50, 100);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    // ==========================================
    // 6. EnsureUserIsProjectMemberAsync (2 tests)
    // ==========================================

    [Fact]
    public async Task EnsureUserIsProjectMember_ShouldSucceed_WhenUserIsMember()
    {
        // Arrange
        int projectId = 100;
        string userId = "user-1";

        var project = new Project
        {
            Id = projectId,
            ProjectUsers = new List<ProjectUser>
            {
                new ProjectUser { ProjectId = projectId, UserId = userId }
            }
        };

        A.CallTo(() => _projectRepo.GetByProjectIdAsync(projectId)).Returns(project);

        // Act & Assert
        await _service.EnsureUserIsProjectMemberAsync(projectId, userId);
    }

    [Fact]
    public async Task EnsureUserIsProjectMember_ShouldThrowForbidden_WhenUserIsNotMember()
    {
        // Arrange
        int projectId = 100;
        var project = new Project
        {
            Id = projectId,
            ProjectUsers = new List<ProjectUser>() // Пустий список
        };

        A.CallTo(() => _projectRepo.GetByProjectIdAsync(projectId)).Returns(project);

        // Act
        Func<Task> act = async () => await _service.EnsureUserIsProjectMemberAsync(projectId, "user-1");

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    // ==========================================
    // 7. EnsureRoleIsValid (2 tests)
    // ==========================================

    [Theory]
    [InlineData("Viewer")]
    [InlineData("Manager")]
    public void EnsureRoleIsValid_ShouldReturnRole_WhenStringIsValid(string roleString)
    {
        // Act
        var result = _service.EnsureRoleIsValid(roleString);

        // Assert

        // 1. Перевіряємо, що таке значення взагалі існує в Enum (найважливіша перевірка)
        result.Should().BeDefined();

        // 2. Перевіряємо, що повернулось правильне значення (конвертуємо рядок в Enum для порівняння)
        var expected = Enum.Parse<ProjectUserRole>(roleString);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("SuperAdmin")] // Припустимо, такої ролі немає
    [InlineData("BadRole")]
    [InlineData("")]
    public void EnsureRoleIsValid_ShouldThrowValidation_WhenStringIsInvalid(string invalidRole)
    {
        // Act
        Action act = () => _service.EnsureRoleIsValid(invalidRole);

        // Assert
        act.Should().Throw<ValidationException>()
           .WithMessage("Invalid role provided.");
    }
}