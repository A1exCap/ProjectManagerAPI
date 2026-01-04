using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;

namespace ProjectManager.IntegrationTests.Common
{
    public static class TestDbContextFactory
    {
        public static async Task<ApplicationDbContext> CreateWithDefaultValues()
        {
            var context = Create(); // ваш існуючий метод

            context.Users.AddRange(
                 new User { Id = "user-123", UserName = "TestUser1" },
                 new User { Id = "another-user", UserName = "TestUser2" }
             );

            context.Projects.AddRange(
                new Project { Name = "Project 1", OwnerId = "user-123", Status = ProjectStatus.Active },
                new Project { Name = "Project 2", OwnerId = "user-123", Status = ProjectStatus.Completed },
                new Project { Name = "Project 3", OwnerId = "another-user", Status = ProjectStatus.Active }
            );
            
            await context.SaveChangesAsync();

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