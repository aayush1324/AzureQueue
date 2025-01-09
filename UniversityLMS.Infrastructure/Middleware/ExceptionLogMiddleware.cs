using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UniversityLMS.Domain.Entities;
using UniversityLMS.Infrastructure.DbContexts;
using UniversityLMS.Application.DTOs;

namespace UniversityLMS.Infrastructure.Middleware
{
    public class ExceptionLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLogMiddleware> _logger;

        public ExceptionLogMiddleware(RequestDelegate next, ILogger<ExceptionLogMiddleware> logger)
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
                // Handle the exception and provide a response
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Default error message and status code
            var statusCode = HttpStatusCode.InternalServerError;
            var response = new ErrorResponseDTO
            {
                Message = "An unexpected error occurred. Please try again later.",
                ErrorDetails = "An internal error occurred." // Optional: More detailed message for internal logs
            };

            // Specific exception handling
            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    response.Message = exception.Message;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    response.Message = exception.Message;
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    break;

                default:
                    // Log unexpected exceptions

                    statusCode = HttpStatusCode.BadRequest;
                    response.Message = exception.Message;
                    _logger.LogError(exception, "An unexpected error occurred.");
                    break;
            }

            // Log the exception to the database asynchronously
            await LogExceptionToDatabaseAsync(context, exception);

            // Prepare the response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Return the error response
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task LogExceptionToDatabaseAsync(HttpContext context, Exception exception)
        {
            // Use dependency injection to get the DbContext from the service provider
            using var scope = context.RequestServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var exceptionLog = new ExceptionLog
            {
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                Source = exception.Source,
                LoggedAt = DateTime.UtcNow,
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                UserId = context.User?.FindFirst("UserID")?.Value
            };

            try
            {
                // Log the exception in the database
                dbContext.ExceptionLogs.Add(exceptionLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception dbEx)
            {
                // Handle any database logging errors
                _logger.LogError(dbEx, "Failed to log exception to the database.");
            }
        }


    }
}

