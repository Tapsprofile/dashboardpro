namespace DashboardPro.Core.Models;

/// <summary>
/// Result of a metric query
/// </summary>
public class MetricResult
{
    public string MetricId { get; set; } = string.Empty;
    public List<MetricDataPoint> DataPoints { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class MetricDataPoint
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
}
