namespace DashboardPro.Core.Models.Dashboard;

/// <summary>
/// Dashboard configuration schema
/// </summary>
public class DashboardConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public List<WidgetConfig> Widgets { get; set; } = new();
    public FilterConfig? Filters { get; set; }
    public LayoutConfig? Layout { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class WidgetConfig
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = string.Empty; // e.g., "funnel", "heatmap", "gauge"
    public string Title { get; set; } = string.Empty;
    public MetricQuery Query { get; set; } = new();
    public Dictionary<string, object> Options { get; set; } = new();
    public PositionConfig Position { get; set; } = new();
}

public class PositionConfig
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; } = 4;
    public int Height { get; set; } = 4;
}

public class FilterConfig
{
    public List<FilterParameter> Parameters { get; set; } = new();
    public bool RealTimeEnabled { get; set; }
    public int RefreshInterval { get; set; } = 30; // seconds
}

public class FilterParameter
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // e.g., "date-range", "dropdown", "text"
    public string Field { get; set; } = string.Empty;
    public object? DefaultValue { get; set; }
    public List<string>? Options { get; set; }
}

public class LayoutConfig
{
    public int Columns { get; set; } = 12;
    public int RowHeight { get; set; } = 100;
    public string Theme { get; set; } = "light";
}
