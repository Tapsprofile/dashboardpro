namespace DashboardPortal.Application.Abstractions.Data;

public sealed record DataSourceRequest(
    string SourceName,
    Stream Content,
    IReadOnlyDictionary<string, string>? Options = null);
