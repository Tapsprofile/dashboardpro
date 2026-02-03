namespace DashboardPro.Core.Models;

/// <summary>
/// Generic data record representing a single parsed data entry
/// </summary>
public class DataRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Fields { get; set; } = new();
    public string ProviderId { get; set; } = string.Empty;
}
