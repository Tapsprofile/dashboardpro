using DashboardPortal.Domain.Entities;

namespace DashboardPortal.Application.Abstractions.Data;

public interface IDataProvider
{
    string Name { get; }

    ValueTask<IReadOnlyCollection<string>> GetAvailableMetricsAsync(
        CancellationToken cancellationToken = default);

    ValueTask<IReadOnlyCollection<string>> GetAvailableDimensionsAsync(
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<LogEntry> ReadAsync(
        DataSourceRequest request,
        CancellationToken cancellationToken = default);

    IQueryable<LogEntry> Query();

    Task BulkUpsertAsync(
        IAsyncEnumerable<LogEntry> entries,
        CancellationToken cancellationToken = default);
}
