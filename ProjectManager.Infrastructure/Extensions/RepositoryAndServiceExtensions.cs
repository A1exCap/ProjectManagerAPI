using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services.DeadlineReminder;
using ProjectManager.Domain.Interfaces.Repositories;
using ProjectManager.Infrastructure.Background;
using ProjectManager.Infrastructure.Persistence;
using ProjectManager.Infrastructure.Repositories.MSSQL;
using ProjectManager.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Infrastructure.Extensions
{
    public static class RepositoryAndServiceExtensions
    {
        public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services)
        {
            services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
            services.AddScoped<IProjectUserRepository, ProjectUserRepository>();
            services.AddScoped<ITaskAttachmentRepository, TaskAttachmentRepository>();        
            services.AddScoped<IProjectDocumentRepository, ProjectDocumentRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IDeadlineReminderService, DeadlineReminderService>();

            services.AddHostedService<DeadlineReminderHostedService>();

            return services;
        }
    }
}
