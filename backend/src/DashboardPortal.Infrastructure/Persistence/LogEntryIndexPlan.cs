namespace DashboardPortal.Infrastructure.Persistence;

public sealed record IndexDefinition(string Name, IReadOnlyList<string> Columns, bool IsUnique = false);

public static class LogEntryIndexPlan
{
    public static readonly IReadOnlyList<IndexDefinition> Recommended = new[]
    {
        new IndexDefinition("IX_LogEntry_Timestamp", new[] { "Timestamp" }),
        new IndexDefinition("IX_LogEntry_StatusCode", new[] { "StatusCode" }),
        new IndexDefinition("IX_LogEntry_UriStem", new[] { "UriStem" }),
        new IndexDefinition("IX_LogEntry_ClientIp", new[] { "ClientIp" }),
        new IndexDefinition("IX_LogEntry_Timestamp_Status", new[] { "Timestamp", "StatusCode" })
    };
}
