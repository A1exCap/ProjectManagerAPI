using System.Threading.RateLimiting;

namespace ProjectManager_API.Extensions
{
    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services)
        {      
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("https://localhost:7037", "http://localhost:5008", "https://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        options => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromSeconds(10),
                            QueueLimit = 0
                        }
                    );
                });
            });

            return services;
        }
    }
}
