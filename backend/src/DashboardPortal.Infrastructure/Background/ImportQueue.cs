using System.Threading.Channels;
using DashboardPortal.Application.Abstractions.Imports;

namespace DashboardPortal.Infrastructure.Background;

public sealed class ImportQueue : IImportQueue
{
    private readonly Channel<ImportRequest> _channel = Channel.CreateUnbounded<ImportRequest>();

    public void Enqueue(ImportRequest request)
    {
        _channel.Writer.TryWrite(request);
    }

    public ValueTask<ImportRequest> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}
