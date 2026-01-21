using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Application.Services.DeadlineReminder;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerAPI.Tests.Services
{
    public class DeadlineReminderServiceTests
    {
        private readonly ILogger<DeadlineReminderService> _logger;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ICreateMessageService _messageService;
        private readonly IUnitOfWork _unitOfWork;

        private readonly DeadlineReminderService _service;

        public DeadlineReminderServiceTests()
        {
            _logger = NullLogger<DeadlineReminderService>.Instance;
            _projectTaskRepository = A.Fake<IProjectTaskRepository>();
            _messageService = A.Fake<ICreateMessageService>();
            _unitOfWork = A.Fake<IUnitOfWork>();

            _service = new DeadlineReminderService(_logger, _projectTaskRepository, _messageService, _unitOfWork);
        }

        [Fact]
        public async Task CheckDeadlines_ShouldSendMessages_And_UpdateReminderTimestamp()
        {
            // Arrange (Підготовка даних)
            var tasks = new List<ProjectTask>
        {
            new ProjectTask
            {
                Id = 1,
                Title = "Task One",
                AssigneeId = "user-1",
                DueDate = DateTime.UtcNow.AddHours(5),
                ReminderSentAt = null
            },
            new ProjectTask
            {
                Id = 2,
                Title = "Task Two",
                AssigneeId = "user-2",
                DueDate = DateTime.UtcNow.AddHours(3),
                ReminderSentAt = null
            }
        };

            // 2. МАГІЯ: Перетворюємо звичайний List на Async-сумісний Mock
            // (Це дозволяє методу .ToListAsync() у сервісі спрацювати без помилок)
            var mockTasks = tasks.BuildMock();

            // 3. Налаштовуємо репозиторій, щоб він повертав наш мок
            A.CallTo(() => _projectTaskRepository.GetTasksWithUpcomingDeadlines(A<TimeSpan>._))
                .Returns(mockTasks);

            // Act (Дія)
            await _service.CheckDeadlinesAsync();

            // Assert (Перевірка)

            // 1. Перевіряємо, що сервіс повідомлень викликався РІВНО 2 рази (для кожного таска)
            A.CallTo(() => _messageService.CreateAsync(
                A<string>._,
                NotificationType.DeadlineReminder,
                A<string>._,
                RelatedEntityType.ProjectTask,
                A<int>._))
                .MustHaveHappenedTwiceExactly();

            // 2. Перевіряємо конкретні параметри для першого таска (опціонально, але корисно)
            A.CallTo(() => _messageService.CreateAsync(
                "user-1",
                NotificationType.DeadlineReminder,
                A<string>.That.Contains("Task One"), // Перевіряємо, що в тексті є назва таска
                RelatedEntityType.ProjectTask,
                1))
                .MustHaveHappened();

            // 3. Перевіряємо, що ми зберегли зміни в БД (SaveAsync викликався 1 раз)
            A.CallTo(() => _unitOfWork.SaveChangesAsync(default))
                .MustHaveHappenedOnceExactly();

            // 4. ПЕРЕВІРКА СТАНУ: Переконуємось, що поле ReminderSentAt оновилося
            tasks[0].ReminderSentAt.Should().NotBeNull();
            tasks[0].ReminderSentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));

            tasks[1].ReminderSentAt.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckDeadlines_ShouldDoNothing_WhenNoTasksFound()
        {
            // Arrange
            var emptyList = new List<ProjectTask>();
            var mockEmpty = emptyList.BuildMock();

            A.CallTo(() => _projectTaskRepository.GetTasksWithUpcomingDeadlines(A<TimeSpan>._))
                .Returns(mockEmpty);

            // Act
            await _service.CheckDeadlinesAsync();

            // Assert
            // Жодне повідомлення не мало відправитись
            A.CallTo(() => _messageService.CreateAsync(A<string>._, A<NotificationType>._, A<string>._, A<RelatedEntityType>._, A<int>._))
                .MustNotHaveHappened();

            // Збереження все одно викликається у твоєму коді, навіть якщо змін не було 
            // (це ок, але можна оптимізувати в сервісі: if (!tasks.Any()) return;)
            A.CallTo(() => _unitOfWork.SaveChangesAsync(default))
                .MustHaveHappenedOnceExactly();
        }
    }
}
