using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using ProjectManager.Infrastructure.Extensions;
using ProjectManager_API.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

IdentityModelEventSource.ShowPII = true;

builder.Services.AddPresentationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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
