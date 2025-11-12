using ProjectManager_API.Common;
using ProjectManager_API.Exceptions;
using System.Net;
using System.Text.Json;

namespace ProjectManager_API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception while processing request {Method} {Path}",
           context.Request?.Method, context.Request?.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                ValidationException => (int)HttpStatusCode.BadRequest,
                UnauthorizedException => (int)HttpStatusCode.Unauthorized,
                ForbiddenException => (int)HttpStatusCode.Forbidden,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = exception switch
            {
                NotFoundException ex => ApiResponseFactory.NotFound<object>(ex.Message),
                ValidationException ex => ApiResponseFactory.BadRequest<object>(ex.Message),
                UnauthorizedException ex => ApiResponseFactory.Unauthorized<object>(ex.Message),
                ForbiddenException ex => ApiResponseFactory.Forbidden<object>(ex.Message),
                _ => ApiResponseFactory.ServerError<object>("An unexpected error occurred.")
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
