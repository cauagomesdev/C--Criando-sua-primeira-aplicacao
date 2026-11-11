using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Adopet.Exceptions.Handlers;

public class PetEmProcessoDeAdocaoExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not PetEmProcessoDeAdocaoException)
        {
            return false;
        }
        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Pet em processo de adoção",
            Status = StatusCodes.Status409Conflict,
            Detail = exception.Message,
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
