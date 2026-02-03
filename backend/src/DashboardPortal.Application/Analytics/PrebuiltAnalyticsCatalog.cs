namespace DashboardPortal.Application.Analytics;

public sealed record AnalyticsDefinition(
    string Key,
    string Category,
    string Description,
    string Metric,
    string? Dimension = null);

public static class PrebuiltAnalyticsCatalog
{
    public static readonly IReadOnlyList<AnalyticsDefinition> Items = new List<AnalyticsDefinition>
    {
        // Performance
        new("perf.avg_latency", "Performance", "Average latency", "avg(time_taken)"),
        new("perf.p95_latency", "Performance", "P95 latency", "p95(time_taken)"),
        new("perf.p99_latency", "Performance", "P99 latency", "p99(time_taken)"),
        new("perf.max_latency", "Performance", "Max latency", "max(time_taken)"),
        new("perf.min_latency", "Performance", "Min latency", "min(time_taken)"),
        new("perf.slowest_endpoints", "Performance", "Slowest endpoints", "avg(time_taken)", "cs_uri_stem"),
        new("perf.throughput_min", "Performance", "Throughput per minute", "count(*)", "minute"),
        new("perf.throughput_hour", "Performance", "Throughput per hour", "count(*)", "hour"),
        new("perf.throughput_day", "Performance", "Throughput per day", "count(*)", "day"),
        new("perf.latency_by_status", "Performance", "Latency by status", "avg(time_taken)", "sc_status"),
        new("perf.latency_by_method", "Performance", "Latency by method", "avg(time_taken)", "cs_method"),
        new("perf.latency_by_ip", "Performance", "Latency by client IP", "avg(time_taken)", "c_ip"),
        new("perf.latency_by_host", "Performance", "Latency by host", "avg(time_taken)", "cs_host"),
        new("perf.response_bytes_avg", "Performance", "Avg response size", "avg(sc_bytes)"),
        new("perf.response_bytes_p95", "Performance", "P95 response size", "p95(sc_bytes)"),
        new("perf.response_bytes_p99", "Performance", "P99 response size", "p99(sc_bytes)"),
        new("perf.request_bytes_avg", "Performance", "Avg request size", "avg(cs_bytes)"),
        new("perf.request_bytes_p95", "Performance", "P95 request size", "p95(cs_bytes)"),
        new("perf.request_bytes_p99", "Performance", "P99 request size", "p99(cs_bytes)"),
        new("perf.latency_heatmap", "Performance", "Latency heatmap", "avg(time_taken)", "hour_of_day"),

        // Traffic
        new("traffic.total_requests", "Traffic", "Total requests", "count(*)"),
        new("traffic.unique_ips", "Traffic", "Unique client IPs", "count(distinct c_ip)"),
        new("traffic.top_ips", "Traffic", "Top client IPs", "count(*)", "c_ip"),
        new("traffic.top_endpoints", "Traffic", "Top endpoints", "count(*)", "cs_uri_stem"),
        new("traffic.top_methods", "Traffic", "Top HTTP methods", "count(*)", "cs_method"),
        new("traffic.status_distribution", "Traffic", "Status distribution", "count(*)", "sc_status"),
        new("traffic.user_agents", "Traffic", "User agent mix", "count(*)", "cs_user_agent"),
        new("traffic.referers", "Traffic", "Top referers", "count(*)", "cs(Referer)"),
        new("traffic.hourly_trend", "Traffic", "Hourly trend", "count(*)", "hour"),
        new("traffic.daily_trend", "Traffic", "Daily trend", "count(*)", "day"),
        new("traffic.weekday_trend", "Traffic", "Weekday trend", "count(*)", "day_of_week"),
        new("traffic.geo_requests", "Traffic", "Geo requests", "count(*)", "geo"),
        new("traffic.geo_latency", "Traffic", "Geo latency", "avg(time_taken)", "geo"),
        new("traffic.site_distribution", "Traffic", "Site distribution", "count(*)", "s_sitename"),
        new("traffic.host_distribution", "Traffic", "Host distribution", "count(*)", "cs_host"),
        new("traffic.port_distribution", "Traffic", "Port distribution", "count(*)", "s_port"),

        // Stability
        new("stability.error_rate", "Stability", "Error rate", "count(sc_status=500)"),
        new("stability.error_rate_4xx", "Stability", "4xx rate", "count(sc_status=404)"),
        new("stability.error_rate_5xx", "Stability", "5xx rate", "count(sc_status=500)"),
        new("stability.error_by_endpoint", "Stability", "Errors by endpoint", "count(sc_status=500)", "cs_uri_stem"),
        new("stability.error_by_host", "Stability", "Errors by host", "count(sc_status=500)", "cs_host"),
        new("stability.error_by_ip", "Stability", "Errors by IP", "count(sc_status=500)", "c_ip"),
        new("stability.substatus_distribution", "Stability", "Substatus distribution", "count(*)", "sc_substatus"),
        new("stability.win32_distribution", "Stability", "Win32 distribution", "count(*)", "sc_win32_status"),
        new("stability.anomaly_5xx", "Stability", "5xx anomaly detection", "anomaly(count(sc_status=500))", "hour"),
        new("stability.anomaly_latency", "Stability", "Latency anomaly detection", "anomaly(avg(time_taken))", "hour"),
        new("stability.outlier_ips", "Stability", "Outlier IPs", "outliers(avg(time_taken))", "c_ip"),
        new("stability.outlier_endpoints", "Stability", "Outlier endpoints", "outliers(avg(time_taken))", "cs_uri_stem"),
        new("stability.retry_ratio", "Stability", "Retry ratio", "count(sc_status=500)", "cs_uri_query"),
        new("stability.slow_errors", "Stability", "Slow error responses", "avg(time_taken)", "sc_status"),
        new("stability.timeouts", "Stability", "Timeouts", "count(sc_status=504)"),

        // Security
        new("security.suspicious_ips", "Security", "Suspicious IPs", "count(*)", "c_ip"),
        new("security.auth_failures", "Security", "Auth failures", "count(sc_status=401)"),
        new("security.forbidden", "Security", "Forbidden requests", "count(sc_status=403)"),
        new("security.path_traversal", "Security", "Path traversal attempts", "count(*)", "cs_uri_stem"),
        new("security.bot_user_agents", "Security", "Bot user agents", "count(*)", "cs_user_agent"),
        new("security.sql_injection", "Security", "SQL injection attempts", "count(*)", "cs_uri_query"),
        new("security.xss_attempts", "Security", "XSS attempts", "count(*)", "cs_uri_query"),
        new("security.brute_force", "Security", "Brute force attempts", "count(sc_status=401)", "c_ip"),
        new("security.status_spikes", "Security", "Status spikes", "anomaly(count(*))", "sc_status"),
        new("security.geo_blocked", "Security", "Blocked by geo", "count(*)", "geo"),
    };
}
