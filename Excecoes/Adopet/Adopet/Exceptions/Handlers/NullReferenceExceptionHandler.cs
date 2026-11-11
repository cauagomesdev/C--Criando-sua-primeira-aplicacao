using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Adopet.Exceptions.Handlers;

public class NullReferenceExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not NullReferenceException)
        {
            return ValueTask.FromResult(false);
        }
        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Falha ao encontrar objeto solicitado",
            Status = StatusCodes.Status404NotFound,
            Detail = exception.Message,
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.WriteAsJsonAsync(problemDetails);
        return ValueTask.FromResult(true);
    }
}
