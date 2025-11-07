using ProjectManager.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using ProjectManager.Application.Extensions;
using ProjectManager_API.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddPresentationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

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
