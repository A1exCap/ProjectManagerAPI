using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Application.Services;
using System.Threading.RateLimiting;

namespace ProjectManager_API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGenConfig();
            services.AddSecurityServices();

            return services;
        }
    }
}
