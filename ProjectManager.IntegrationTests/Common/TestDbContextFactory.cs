using Microsoft.EntityFrameworkCore;
using ProjectManager.Infrastructure.Persistence; // твій AppDbContext namespace
using System;

namespace ProjectManager.IntegrationTests.Common
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // нова БД для кожного тесту
                .Options;

            var context = new ApplicationDbContext(options);

            // Створюємо таблиці у InMemory
            context.Database.EnsureCreated();

            return context;
        }
    }
}