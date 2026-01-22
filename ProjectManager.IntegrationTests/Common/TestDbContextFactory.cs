using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;
using System.Runtime.Intrinsics.X86;

namespace ProjectManager.IntegrationTests.Common
{
    public static class TestDbContextFactory
    {
        public static async Task<ApplicationDbContext> CreateWithDefaultValues()
        {
            var context = Create();

            var user1 = new User { Id = "user-123", UserName = "TestUser1" };
            var user2 = new User { Id = "another-user", UserName = "TestUser2" };

            context.Users.AddRange(user1, user2);

            var project1 = new Project { Name = "Project 1", OwnerId = user1.Id, Status = ProjectStatus.Active };
            var project2 = new Project { Name = "Project 2", OwnerId = user1.Id, Status = ProjectStatus.Completed };
            var project3 = new Project { Name = "Project 3", OwnerId = user2.Id, Status = ProjectStatus.Active };

            context.Projects.AddRange(project1, project2, project3);

            context.ProjectUser.AddRange(
                new ProjectUser { Project = project1, UserId = user1.Id, Role = ProjectUserRole.Owner },
                new ProjectUser { Project = project1, UserId = user2.Id, Role = ProjectUserRole.Contributor }
            );

            await context.SaveChangesAsync(); // Тут всі ID згенеруються і зв'яжуться автоматично

            return context;
        }
        public static ApplicationDbContext Create()
        {
            // 1. Створюємо з'єднання з параметром :memory:
            var connection = new SqliteConnection("DataSource=:memory:");

            // 2. ВІДКРИВАЄМО з'єднання обов'язково! 
            // Без цього база не буде існувати при створенні контексту.
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection) // Використовуємо SQLite замість InMemory
                .Options;

            var context = new ApplicationDbContext(options);

            // 3. Створюємо схему таблиць
            context.Database.EnsureCreated();

            return context;
        }
    }
}