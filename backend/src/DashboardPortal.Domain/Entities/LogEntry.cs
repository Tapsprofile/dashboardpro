namespace DashboardPortal.Domain.Entities;

public sealed class LogEntry
{
    public long Id { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? ClientIp { get; set; }
    public string? ServerIp { get; set; }
    public string? ServerPort { get; set; }
    public string? SiteName { get; set; }
    public string? Host { get; set; }
    public string? Method { get; set; }
    public string? UriStem { get; set; }
    public string? UriQuery { get; set; }
    public string? Username { get; set; }
    public string? UserAgent { get; set; }
    public string? Referrer { get; set; }
    public int? StatusCode { get; set; }
    public int? SubStatusCode { get; set; }
    public int? Win32Status { get; set; }
    public long? TimeTakenMs { get; set; }
    public long? BytesSent { get; set; }
    public long? BytesReceived { get; set; }

    public Dictionary<string, string?> CustomProperties { get; set; } = new();
}
