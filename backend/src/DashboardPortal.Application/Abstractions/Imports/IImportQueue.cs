namespace DashboardPortal.Application.Abstractions.Imports;

public interface IImportQueue
{
    void Enqueue(ImportRequest request);
    ValueTask<ImportRequest> DequeueAsync(CancellationToken cancellationToken);
}

public sealed record ImportRequest(string SourceName, string FilePath);
