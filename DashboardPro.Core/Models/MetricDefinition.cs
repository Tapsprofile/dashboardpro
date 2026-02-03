namespace DashboardPro.Core.Models;

/// <summary>
/// Defines available metrics and their properties
/// </summary>
public class MetricDefinition
{
    public string MetricId { get; set; } = string.Empty;
    public string MetricName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MetricType Type { get; set; }
    public List<string> AvailableDimensions { get; set; } = new();
    public List<string> AvailableAggregations { get; set; } = new();
}

public enum MetricType
{
    Counter,
    Gauge,
    Histogram,
    Rate
}
