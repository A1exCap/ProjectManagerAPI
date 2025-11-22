using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Services.DeadlineReminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Background
{
    public class DeadlineReminderHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DeadlineReminderHostedService> _logger;

        public DeadlineReminderHostedService(
            IServiceProvider serviceProvider,
            ILogger<DeadlineReminderHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reminderService = scope.ServiceProvider
                            .GetRequiredService<IDeadlineReminderService>();

                        await reminderService.CheckDeadlinesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DeadlineReminderHostedService");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
