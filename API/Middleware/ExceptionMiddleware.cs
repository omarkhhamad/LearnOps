using System.Text.Json;
using Application.Bases;
using Microsoft.AspNetCore.Mvc;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled Exception");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                // await context.Response.WriteAsync(JsonSerializer.Serialize(Result<string>.Fail(ex.Message, 500)));
                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problem);
            }
        }
    }

}
