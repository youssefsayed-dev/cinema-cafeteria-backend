using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace BusinessManagement.Api.Middlewares;

public class JsonExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JsonExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public JsonExceptionHandlingMiddleware(RequestDelegate next, ILogger<JsonExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        // Enable buffering so we can read the body multiple times
        context.Request.EnableBuffering();

        string rawBody = string.Empty;
        try
        {
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
            {
                rawBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // reset for downstream
            }

            await _next(context);
        }
        catch (JsonException jex)
        {
            _logger.LogWarning(jex, "JSON parsing error for request {Method} {Path}: {Message}. RawBody: {RawBody}", context.Request.Method, context.Request.Path, jex.Message, Truncate(rawBody, 2000));

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new {
                message = "Malformed JSON or invalid field format.",
                detail = _env.IsDevelopment() ? jex.Message : null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (FormatException fex)
        {
            _logger.LogWarning(fex, "Format error for request {Method} {Path}: {Message}. RawBody: {RawBody}", context.Request.Method, context.Request.Path, fex.Message, Truncate(rawBody, 2000));

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new {
                message = "Invalid value format in request.",
                detail = _env.IsDevelopment() ? fex.Message : null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private string Truncate(string s, int max) => string.IsNullOrEmpty(s) ? s : (s.Length <= max ? s : s.Substring(0, max) + "... [truncated]");
}
