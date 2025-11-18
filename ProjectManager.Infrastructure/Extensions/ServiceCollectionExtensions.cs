using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Application.Services;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;
using System.Text;

namespace ProjectManager.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddDatabase(config);
            services.AddIdentityServices();
            services.AddJwtAuthentication(config);
            services.AddRepositoriesAndServices(); 
            return services;
        }
    }
}
