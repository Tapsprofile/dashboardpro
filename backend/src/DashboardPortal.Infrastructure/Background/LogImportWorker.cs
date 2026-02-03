using DashboardPortal.Application.Abstractions.Data;
using DashboardPortal.Application.Abstractions.Imports;
using Microsoft.Extensions.Hosting;

namespace DashboardPortal.Infrastructure.Background;

public sealed class LogImportWorker : BackgroundService
{
    private readonly IImportQueue _queue;
    private readonly IDataProvider _dataProvider;

    public LogImportWorker(IImportQueue queue, IDataProvider dataProvider)
    {
        _queue = queue;
        _dataProvider = dataProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var request = await _queue.DequeueAsync(stoppingToken);
            await using var stream = File.OpenRead(request.FilePath);
            var entries = _dataProvider.ReadAsync(
                new DataSourceRequest(request.SourceName, stream),
                stoppingToken);

            await _dataProvider.BulkUpsertAsync(entries, stoppingToken);
            File.Delete(request.FilePath);
        }
    }
}
