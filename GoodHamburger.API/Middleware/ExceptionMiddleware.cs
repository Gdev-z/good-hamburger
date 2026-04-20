using System.Net;
using System.Text.Json;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");
            await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain rule violation");
            await WriteErrorAsync(context, HttpStatusCode.UnprocessableEntity, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                "Ocorreu um erro interno. Tente novamente mais tarde.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.StatusCode  = (int)status;
        context.Response.ContentType = "application/json";

        var payload = JsonSerializer.Serialize(new { error = message });
        await context.Response.WriteAsync(payload);
    }
}
