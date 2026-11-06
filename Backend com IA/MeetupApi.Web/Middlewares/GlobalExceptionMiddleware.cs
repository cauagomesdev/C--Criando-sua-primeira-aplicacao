using System.Text.Json;
using MeetupApi.Web.Dtos;
using MeetupApi.Web.Exceptions;

namespace MeetupApi.Web.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (ResourceNotFoundException ex)
        {
            _logger.LogWarning("Recurso não encontrado: {Mensagem}", ex.Message);
            await WriteErrorAsync(context, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning("Conflito de negócio: {Mensagem}", ex.Message);
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Argumento inválido: {Mensagem}", ex.Message);
            await WriteErrorAsync(context, StatusCodes.Status422UnprocessableEntity, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, "Erro interno do servidor");
        }
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string mensagem)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new ErrorResponse(mensagem), new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return context.Response.WriteAsync(body);
    }
}
