using ComplianceBot.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace ComplianceBot.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// Catches exceptions and returns appropriate HTTP responses
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        var (statusCode, message, errors) = exception switch
        {
            EntityNotFoundException => (HttpStatusCode.NotFound, exception.Message, null),
            UnauthorizedTenantAccessException => (HttpStatusCode.Forbidden, exception.Message, null),
            ValidationException validationEx => (HttpStatusCode.BadRequest, "Validation failed", validationEx.Errors),
            FluentValidation.ValidationException fluentEx => (HttpStatusCode.BadRequest, "Validation failed",
                fluentEx.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())),
            _ => (HttpStatusCode.InternalServerError, "An internal error occurred", null)
        };

        _logger.LogError(exception, "Error occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            message,
            errors
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
