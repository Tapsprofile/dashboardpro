namespace DashboardPortal.Application.Abstractions.Analytics;

public sealed record DataPoint(string Key, double Value);

public sealed record WidgetResult(string WidgetId, IReadOnlyList<DataPoint> Points);
