namespace DashboardPortal.Domain.Configurations;

public sealed class DashboardConfiguration
{
    public string DashboardTitle { get; set; } = string.Empty;
    public List<WidgetConfiguration> Widgets { get; set; } = new();
}

public sealed class WidgetConfiguration
{
    public string Id { get; set; } = string.Empty;
    public WidgetType Type { get; set; }
    public string? Metric { get; set; }
    public string? Dimension { get; set; }
    public string? Label { get; set; }
    public string? X { get; set; }
    public string? Y { get; set; }
    public string? Granularity { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public List<string>? Steps { get; set; }
    public List<string>? Columns { get; set; }
    public string? Sort { get; set; }
    public Dictionary<string, string>? Options { get; set; }
}

public enum WidgetType
{
    KpiCard,
    Gauge,
    LineChart,
    AreaChart,
    BarChart,
    PieChart,
    DonutChart,
    ScatterPlot,
    Heatmap,
    Funnel,
    DataTable,
    GeoMap,
    Histogram,
    BoxPlot,
    Radar,
    TreeMap,
    Sankey,
    Sparkline,
    Bullet,
    CalendarHeatmap,
    StackedBar
}
