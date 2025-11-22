using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.CreateMessage;
using ProjectManager.Domain.Entities;
using ProjectManager.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Services.DeadlineReminder
{
    public class DeadlineReminderService : IDeadlineReminderService
    {
        private readonly ILogger<DeadlineReminderService> _logger;
        private readonly IProjectTaskRepository _projectTaskRepository;
        private readonly ICreateMessageService _messageService;
        private readonly IUnitOfWork _unitOfWork;   
        public DeadlineReminderService(ILogger<DeadlineReminderService> logger, IProjectTaskRepository projectTaskRepository,
            ICreateMessageService messageService, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _messageService = messageService;
            _projectTaskRepository = projectTaskRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task CheckDeadlinesAsync()
        {
            _logger.LogInformation("Checking deadlines for upcoming tasks...");

            var tasks = await _projectTaskRepository.GetTasksWithUpcomingDeadlines(TimeSpan.FromDays(1)).ToListAsync();

            foreach(var task in tasks)
            {
                await _messageService.CreateAsync(task.AssigneeId, NotificationType.DeadlineReminder, 
                    $"Reminder: The task '{task.Title}' is due on {task.DueDate?.ToString("yyyy-MM-dd HH:mm")} UTC.", 
                    RelatedEntityType.ProjectTask, task.Id);

                task.ReminderSentAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
