namespace DashboardPro.Core.Models;

/// <summary>
/// Query parameters for metric retrieval
/// </summary>
public class MetricQuery
{
    public string MetricId { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<string> Dimensions { get; set; } = new();
    public string Aggregation { get; set; } = "sum";
    public Dictionary<string, string> Filters { get; set; } = new();
    public int? Limit { get; set; }
    public string GroupBy { get; set; } = string.Empty;
}
