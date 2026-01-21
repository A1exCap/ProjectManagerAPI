using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.DTOs.Project;
using ProjectManager.Application.Features.Projects.Commands.CreateProjectCommand;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ProjectManagerAPI.Tests.Features.Projects
{
    public class CreateProjectCommandHandlerTests
    {
        // --- Це залежності handler-а ---
        private readonly ILogger<CreateProjectCommandHandler> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectUserRepository _projectUserRepository;
        private readonly IUnitOfWork _unitOfWork;

        // --- Тестований handler ---
        private readonly CreateProjectCommandHandler _handler;

        public CreateProjectCommandHandlerTests()
        {
            // Fake Logger. Ми його не тестуємо, просто передаємо handler-у
            _logger = NullLogger<CreateProjectCommandHandler>.Instance;

            // Fake репозиторії. Вони не роблять реальних викликів у базу.
            _projectRepository = A.Fake<IProjectRepository>();
            _projectUserRepository = A.Fake<IProjectUserRepository>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            // Створюємо handler з фейками
            _handler = new CreateProjectCommandHandler(
                _logger,
                _projectUserRepository,
                _projectRepository,
                _unitOfWork
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateProjectAndOwner()
        {
            // ------------------
            // ARRANGE (Підготовка)
            // ------------------

            var userId = "user-123";

            // Це дані, які користувач передає для створення проекту
            var dto = new CreateProjectDto
            {
                Name = "Test Project",
                Description = "Description",
                EndDate = DateTime.UtcNow.AddDays(30),
                Status = ProjectStatus.Active,
                Visibility = ProjectVisibility.Private,
                ClientName = "Client",
                Budget = 1000,
                Technologies = "C#; .Net"
            };

            // Команда, яку ми будемо передавати handler-у
            var command = new CreateProjectCommand(userId, dto);

            // Ми хочемо перевірити, що всередині handler-а створюється проект
            Project? savedProject = null;

            // Fake виклик AddProjectAsync
            // A<Project>._ = будь-який об'єкт типу Project
            // Invokes = коли метод викликається, перехоплюємо об'єкт і встановлюємо Id
            A.CallTo(() => _projectRepository.AddProjectAsync(A<Project>._))
                .Invokes((Project p) =>
                {
                    // Імітуємо EF Core: виставляємо Id після збереження
                    p.Id = 42;
                    savedProject = p; // Зберігаємо проект назовні для перевірок
                })
                .Returns(Task.CompletedTask); // Метод async, повертаємо завершену задачу

            // ------------------
            // ACT (Виконання)
            // ------------------

            var result = await _handler.Handle(command, CancellationToken.None);

            // ------------------
            // ASSERT (Перевірки)
            // ------------------

            // 1️⃣ Перевіряємо, що handler повернув правильний Id
            result.Should().Be(42);

            // 2️⃣ Перевіряємо, що всередині handler-а створився проект з правильними даними
            savedProject.Should().NotBeNull();           // Проект створений
            savedProject!.Name.Should().Be(dto.Name);   // Name співпадає
            savedProject.OwnerId.Should().Be(userId);   // OwnerId співпадає
            savedProject.Visibility.Should().Be(dto.Visibility);

            // 3️⃣ Перевіряємо, що додано користувача з роллю Owner

            A.CallTo(() => _projectRepository.AddProjectAsync(A<Project>._))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _projectUserRepository.AddProjectUserAsync(
                    A<ProjectUser>.That.Matches(pu =>
                        pu.ProjectId == 42 &&
                        pu.UserId == userId &&
                        pu.Role == ProjectUserRole.Owner)))
                .MustHaveHappenedOnceExactly(); // Перевірка: виклик був точно 1 раз

            // 4️⃣ Перевіряємо, що SaveChangesAsync викликано двічі
            //  - після створення Project
            //  - після додавання ProjectUser
            A.CallTo(() => _unitOfWork.SaveChangesAsync(CancellationToken.None))
                .MustHaveHappenedOnceExactly();
        }
    }
}