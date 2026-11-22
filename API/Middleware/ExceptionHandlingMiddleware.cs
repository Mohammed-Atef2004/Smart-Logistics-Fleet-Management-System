using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // FluentValidation throws when a command fails validation. That is
        // a client error, not a server error — map it to 400 with the
        // field-level messages instead of returning an opaque 500.
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var validationProblem = new ValidationProblemDetails(
                validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "Validation.Error",
                Title = "Validation Error",
                Detail = "One or more validation errors occurred."
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(validationProblem));
        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "Server.Error",
            Title = "Server Error",
            Detail = "An unexpected error occurred on our side. Please try again later."
        };

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
