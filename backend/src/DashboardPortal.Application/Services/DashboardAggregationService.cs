using System.Globalization;
using System.Linq.Expressions;
using DashboardPortal.Application.Abstractions.Analytics;
using DashboardPortal.Application.Abstractions.Data;
using DashboardPortal.Domain.Configurations;
using DashboardPortal.Domain.Entities;

namespace DashboardPortal.Application.Services;

public sealed class DashboardAggregationService : IDashboardAggregationService
{
    private readonly IDataProvider _dataProvider;

    public DashboardAggregationService(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public Task<IReadOnlyList<WidgetResult>> RunAsync(
        DashboardConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var query = _dataProvider.Query();
        var results = configuration.Widgets
            .Select(widget => ExecuteWidget(query, widget))
            .ToList();

        return Task.FromResult<IReadOnlyList<WidgetResult>>(results);
    }

    private static WidgetResult ExecuteWidget(
        IQueryable<LogEntry> query,
        WidgetConfiguration widget)
    {
        if (string.IsNullOrWhiteSpace(widget.Dimension))
        {
            var metricValue = EvaluateMetric(query, widget.Metric);
            return new WidgetResult(widget.Id, new List<DataPoint>
            {
                new(widget.Label ?? widget.Metric ?? "value", metricValue)
            });
        }

        var dimensionSelector = BuildSelector(widget.Dimension);
        var grouped = query.GroupBy(dimensionSelector);
        var points = grouped
            .Select(group => new DataPoint(
                Convert.ToString(group.Key, CultureInfo.InvariantCulture) ?? "unknown",
                EvaluateMetric(group.AsQueryable(), widget.Metric)))
            .ToList();

        return new WidgetResult(widget.Id, points);
    }

    private static Expression<Func<LogEntry, object?>> BuildSelector(string dimension)
    {
        var parameter = Expression.Parameter(typeof(LogEntry), "entry");

        Expression access = dimension switch
        {
            "hour" or "hour_of_day" => Expression.Property(
                Expression.Property(parameter, nameof(LogEntry.Timestamp)),
                nameof(DateTimeOffset.Hour)),
            "minute" => Expression.Property(
                Expression.Property(parameter, nameof(LogEntry.Timestamp)),
                nameof(DateTimeOffset.Minute)),
            "day" => Expression.Property(
                Expression.Property(parameter, nameof(LogEntry.Timestamp)),
                nameof(DateTimeOffset.Date)),
            "day_of_week" => Expression.Property(
                Expression.Property(parameter, nameof(LogEntry.Timestamp)),
                nameof(DateTimeOffset.DayOfWeek)),
            _ => Expression.Property(parameter, ResolveField(dimension))
        };

        var converted = Expression.Convert(access, typeof(object));
        return Expression.Lambda<Func<LogEntry, object?>>(converted, parameter);
    }

    private static string ResolveField(string dimension)
    {
        return dimension switch
        {
            "timestamp" => nameof(LogEntry.Timestamp),
            "c_ip" => nameof(LogEntry.ClientIp),
            "cs_uri_stem" => nameof(LogEntry.UriStem),
            "cs_uri_query" => nameof(LogEntry.UriQuery),
            "cs_method" => nameof(LogEntry.Method),
            "cs_user_agent" => nameof(LogEntry.UserAgent),
            "cs_username" => nameof(LogEntry.Username),
            "sc_status" => nameof(LogEntry.StatusCode),
            "sc_substatus" => nameof(LogEntry.SubStatusCode),
            "sc_win32_status" => nameof(LogEntry.Win32Status),
            "time_taken" => nameof(LogEntry.TimeTakenMs),
            "cs_host" => nameof(LogEntry.Host),
            "s_ip" => nameof(LogEntry.ServerIp),
            "s_port" => nameof(LogEntry.ServerPort),
            "s_sitename" => nameof(LogEntry.SiteName),
            _ => throw new InvalidOperationException($"Unsupported dimension: {dimension}")
        };
    }

    private static double EvaluateMetric(IQueryable<LogEntry> query, string? metric)
    {
        if (string.IsNullOrWhiteSpace(metric))
        {
            return query.Count();
        }

        metric = metric.Trim().ToLowerInvariant();

        if (metric == "count(*)")
        {
            return query.Count();
        }

        if (metric.StartsWith("avg(") && metric.EndsWith(")"))
        {
            var field = metric[4..^1];
            return query.Average(entry => ExtractNumeric(entry, field));
        }

        if (metric.StartsWith("count(") && metric.EndsWith(")"))
        {
            var predicate = metric[6..^1];
            if (predicate.Contains('='))
            {
                var parts = predicate.Split('=', 2, StringSplitOptions.TrimEntries);
                return query.Count(entry => FieldEquals(entry, parts[0], parts[1]));
            }

            return query.Count();
        }

        throw new InvalidOperationException($"Unsupported metric: {metric}");
    }

    private static double ExtractNumeric(LogEntry entry, string field)
    {
        return field switch
        {
            "time_taken" => entry.TimeTakenMs ?? 0,
            "sc_bytes" => entry.BytesSent ?? 0,
            "cs_bytes" => entry.BytesReceived ?? 0,
            _ => 0
        };
    }

    private static bool FieldEquals(LogEntry entry, string field, string value)
    {
        return field switch
        {
            "sc_status" => entry.StatusCode?.ToString(CultureInfo.InvariantCulture) == value,
            "cs_method" => string.Equals(entry.Method, value, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }
}
