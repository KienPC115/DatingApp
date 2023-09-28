using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware
{
    // a function that can process HTTP request
    // this is represent for the next middleware
    private readonly RequestDelegate _next; 

    // Logger: to log the exception 
    private readonly ILogger<ExceptionMiddleware> _logger;

    // IHostEnvironment: allow us to see whether we're running in development mode or in production mode.
    private readonly IHostEnvironment _env;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    // recognize this is middleware in the our frameworke, and the framework expect to see a method called Invoke Async
    public async Task InvokeAsync(HttpContext context) {
        try
        {
            await _next(context);
        }
        catch(Exception ex) {
            // If they are not handling the exception before it gets to this one, then it is going to hit this particular.
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");
        
            var options = new JsonSerializerOptions{
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
        
            await context.Response.WriteAsync(json);
        }
    }
}