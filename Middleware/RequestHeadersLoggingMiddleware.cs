using System.Text;

namespace AspMvcMiddleWare.Middleware;

public class RequestHeadersLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _logFilePath;

    public RequestHeadersLoggingMiddleware(RequestDelegate next, IWebHostEnvironment env)
    {
        _next = next;

        var logsDir = Path.Combine(env.ContentRootPath, "Logs");
        Directory.CreateDirectory(logsDir);
        _logFilePath = Path.Combine(logsDir, "request-headers.txt");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== {DateTime.Now:yyyy-MM-dd HH:mm:ss} | {context.Request.Method} {context.Request.Path}{context.Request.QueryString} ===");

        foreach (var header in context.Request.Headers)
        {
            sb.AppendLine($"{header.Key}: {header.Value}");
        }

        sb.AppendLine();

        await File.AppendAllTextAsync(_logFilePath, sb.ToString(), Encoding.UTF8);

        await _next(context);
    }
}