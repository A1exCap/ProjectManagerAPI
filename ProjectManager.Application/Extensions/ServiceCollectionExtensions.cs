using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Application.Services;
using ProjectManager.Application.Services.Access;
using ProjectManager.Application.Services.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITaskValidationService, EntityValidationService>();
            services.AddScoped<IProjectAccessService, ProjectAccessService>();
            return services;
        }
    }
}
