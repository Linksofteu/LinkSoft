using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Text;
using LinkSoft.ERMS.Interfaces;
using LinkSoft.ERMS.Models;

namespace LinkSoft.ERMS.Options;

public class SoapLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IErmsLogger _logger;
    private readonly string _path;

    private const string Base64ElementName = "dmEncodedContent";

    private static readonly Regex Base64SanitizerRegex = new(
        $@"<([a-zA-Z0-9]*:)?{Base64ElementName}>([A-Za-z0-9+/=\r\n\s]{{1000,}})</\1?{Base64ElementName}>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public SoapLoggingMiddleware(RequestDelegate next, IErmsLogger logger, string path)
    {
        _next = next;
        _logger = logger;
        _path = path;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.Equals(_path, StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        SoapLogEntry logEntry = new()
        {
            Direction = ErmsLogDirection.Input,
            Timestamp = DateTime.UtcNow,
            Path = context.Request.Path,
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Username = context.User?.Identity?.IsAuthenticated == true
                ? context.User.Identity.Name
                : null
        };

        try
        {
            // Načtení a sanitizace requestu
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var rawRequest = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            logEntry.RequestXml = Base64SanitizerRegex.Replace(
                rawRequest,
                $"<$1{Base64ElementName}>[BASE64_REPLACED]</$1{Base64ElementName}>");
        }
        catch (Exception ex)
        {
            logEntry.RequestXml = "[REQUEST READ FAILED]";
            logEntry.ExceptionMessage = $"Request read error: {ex.Message}";
            await SafeLogAsync(logEntry);
            throw;
        }

        var originalResponseBody = context.Response.Body;
        await using var newResponseBody = new MemoryStream();
        context.Response.Body = newResponseBody;

        try
        {
            await _next(context);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            logEntry.ResponseXml = await new StreamReader(newResponseBody).ReadToEndAsync();
            logEntry.ResponseStatusCode = context.Response.StatusCode;

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalResponseBody);
        }
        catch (Exception ex)
        {
            logEntry.ExceptionMessage = ex.ToString();
            logEntry.ResponseStatusCode = 500;
            throw;
        }
        finally
        {
            context.Response.Body = originalResponseBody;
            await SafeLogAsync(logEntry);
        }
    }

    private async Task SafeLogAsync(SoapLogEntry entry)
    {
        try
        {
            await _logger.LogSoapExchangeAsync(entry);
        }
        catch (Exception logEx)
        {
            Console.Error.WriteLine($"[SOAP LOGGING ERROR]: {logEx}");
        }
    }
}