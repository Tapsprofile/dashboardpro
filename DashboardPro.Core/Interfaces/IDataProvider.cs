using DashboardPro.Core.Models;

namespace DashboardPro.Core.Interfaces;

/// <summary>
/// Domain-agnostic data provider interface for pluggable data sources
/// </summary>
public interface IDataProvider
{
    /// <summary>
    /// Unique identifier for the data provider
    /// </summary>
    string ProviderId { get; }
    
    /// <summary>
    /// Display name for the data provider
    /// </summary>
    string ProviderName { get; }
    
    /// <summary>
    /// Parse data from source asynchronously
    /// </summary>
    Task<IEnumerable<DataRecord>> ParseDataAsync(Stream dataStream, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Bulk parse multiple data sources
    /// </summary>
    Task<IEnumerable<DataRecord>> BulkParseAsync(IEnumerable<Stream> dataStreams, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get available metrics from this data provider
    /// </summary>
    Task<IEnumerable<MetricDefinition>> GetAvailableMetricsAsync();
    
    /// <summary>
    /// Get aggregated metric data based on query
    /// </summary>
    Task<MetricResult> GetMetricAsync(MetricQuery query, CancellationToken cancellationToken = default);
}
