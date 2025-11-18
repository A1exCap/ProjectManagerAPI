using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Asn1.X509.Qualified;
using ProjectManager.Application.Abstractions.Services;
using ProjectManager.Application.Common.Interfaces;
using ProjectManager.Application.Services;
using System.Reflection;
using System.Threading.RateLimiting;

namespace ProjectManager_API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services)
        {
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(IUnitOfWork).Assembly);
            });
            services.AddControllers();
            services.AddSwaggerGenConfig();
            services.AddSecurityServices();

            services.AddScoped(x =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                return new BlobServiceClient(config["AzureStorage:ConnectionString"]);
            });

            return services;
        }
    }
}
