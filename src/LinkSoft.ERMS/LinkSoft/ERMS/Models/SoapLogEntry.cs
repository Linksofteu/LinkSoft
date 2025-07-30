namespace LinkSoft.ERMS.Models;

public class SoapLogEntry
{
    public DateTime Timestamp { get; set; }
    public string? Username { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string RequestXml { get; set; } = string.Empty;
    public string ResponseXml { get; set; } = string.Empty;
    public int? ResponseStatusCode { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? Path { get; set; }

    public ErmsLogDirection Direction { get; set; }
}

public enum ErmsLogDirection
{
    Input,
    Output
}