using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using ProjectManager.Infrastructure.Extensions;
using ProjectManager_API.Extensions;
using ProjectManager_API.Middlewares;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() 
    .WriteTo.Console() 
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) 
    .Enrich.FromLogContext() 
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddPresentationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseAuthentication();    
app.UseAuthorization();

app.MapControllers();

app.Run();
