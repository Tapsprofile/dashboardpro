using DashboardPortal.Application.Abstractions.Data;
using DashboardPortal.Domain.Entities;
using DashboardPortal.Infrastructure.Parsing;

namespace DashboardPortal.Infrastructure.Providers;

public sealed class IisLogProvider : IDataProvider
{
    private readonly IisW3cLogParser _parser;
    private readonly List<LogEntry> _inMemoryStore = new();

    public IisLogProvider(IisW3cLogParser parser)
    {
        _parser = parser;
    }

    public string Name => "IIS W3C Extended Logs";

    public ValueTask<IReadOnlyCollection<string>> GetAvailableMetricsAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<string> metrics = new[]
        {
            "count(*)",
            "avg(time_taken)",
            "count(sc_status=500)",
            "count(sc_status=404)",
            "avg(sc_bytes)"
        };

        return ValueTask.FromResult(metrics);
    }

    public ValueTask<IReadOnlyCollection<string>> GetAvailableDimensionsAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<string> dimensions = new[]
        {
            "timestamp",
            "c_ip",
            "cs_uri_stem",
            "cs_method",
            "cs_user_agent",
            "sc_status",
            "s_sitename"
        };

        return ValueTask.FromResult(dimensions);
    }

    public IAsyncEnumerable<LogEntry> ReadAsync(
        DataSourceRequest request,
        CancellationToken cancellationToken = default)
    {
        return _parser.ParseAsync(request.Content, cancellationToken);
    }

    public IQueryable<LogEntry> Query()
    {
        return _inMemoryStore.AsQueryable();
    }

    public async Task BulkUpsertAsync(
        IAsyncEnumerable<LogEntry> entries,
        CancellationToken cancellationToken = default)
    {
        await foreach (var entry in entries.WithCancellation(cancellationToken))
        {
            _inMemoryStore.Add(entry);
        }
    }
}
