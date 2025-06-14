using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Adapter.Api.ExceptionHandlers;
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "An error occurred while processing the request. The TraceId is: {@traceId}. The Machine is: {@machine}",
            traceId,
            Environment.MachineName
        );

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred on the server. We're working on it...",
            Detail = exception.Message,
            Type = "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
            Extensions =
        {
            ["traceId"] = traceId,
            ["machine"] = Environment.MachineName,
            ["requestId"] = httpContext.Request.Headers.RequestId.ToString()
        }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}