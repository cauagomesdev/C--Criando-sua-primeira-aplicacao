using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Adopet.Exceptions.Handlers;

public class TutorComLimiteAtingidoExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not TutorComLimiteAtingidoException)
        {
            return false;
        }
        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Limite de adoções atingido",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = exception.Message,
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
