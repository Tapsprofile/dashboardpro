using DashboardPro.Core.Interfaces;
using DashboardPro.Core.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DashboardPro.Modules.IISLogs.Parsers;

/// <summary>
/// W3C IIS Log Parser implementation
/// </summary>
public class W3CLogParser : IDataProvider
{
    public string ProviderId => "iis-w3c-logs";
    public string ProviderName => "IIS W3C Log Parser";

    private readonly List<DataRecord> _cachedRecords = new();

    public async Task<IEnumerable<DataRecord>> ParseDataAsync(Stream dataStream, CancellationToken cancellationToken = default)
    {
        var records = new List<DataRecord>();
        using var reader = new StreamReader(dataStream);
        
        List<string>? fields = null;
        string? line;

        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (line.StartsWith("#Fields:"))
            {
                // Extract field names from header
                fields = line.Substring(9).Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            else if (!line.StartsWith("#") && fields != null)
            {
                // Parse data line
                var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (values.Length >= fields.Count)
                {
                    var record = ParseLogEntry(fields, values);
                    records.Add(record);
                }
            }
        }

        _cachedRecords.AddRange(records);
        return records;
    }

    public async Task<IEnumerable<DataRecord>> BulkParseAsync(IEnumerable<Stream> dataStreams, CancellationToken cancellationToken = default)
    {
        var tasks = dataStreams.Select(stream => ParseDataAsync(stream, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results.SelectMany(r => r);
    }

    public Task<IEnumerable<MetricDefinition>> GetAvailableMetricsAsync()
    {
        var metrics = new List<MetricDefinition>
        {
            new MetricDefinition
            {
                MetricId = "request-count",
                MetricName = "Request Count",
                Description = "Total number of requests",
                Type = MetricType.Counter,
                AvailableDimensions = new List<string> { "method", "uri-stem", "status" },
                AvailableAggregations = new List<string> { "count", "sum" }
            },
            new MetricDefinition
            {
                MetricId = "response-time",
                MetricName = "Response Time",
                Description = "Time taken to process requests",
                Type = MetricType.Histogram,
                AvailableDimensions = new List<string> { "method", "uri-stem" },
                AvailableAggregations = new List<string> { "avg", "min", "max", "p95", "p99" }
            },
            new MetricDefinition
            {
                MetricId = "bytes-sent",
                MetricName = "Bytes Sent",
                Description = "Total bytes sent to clients",
                Type = MetricType.Counter,
                AvailableDimensions = new List<string> { "uri-stem", "status" },
                AvailableAggregations = new List<string> { "sum", "avg" }
            },
            new MetricDefinition
            {
                MetricId = "error-rate",
                MetricName = "Error Rate",
                Description = "Rate of 4xx/5xx errors",
                Type = MetricType.Rate,
                AvailableDimensions = new List<string> { "status", "uri-stem" },
                AvailableAggregations = new List<string> { "rate", "count" }
            }
        };

        return Task.FromResult<IEnumerable<MetricDefinition>>(metrics);
    }

    public Task<MetricResult> GetMetricAsync(MetricQuery query, CancellationToken cancellationToken = default)
    {
        var result = new MetricResult
        {
            MetricId = query.MetricId
        };

        var filteredRecords = _cachedRecords.AsEnumerable();

        // Apply time filters
        if (query.StartTime.HasValue)
            filteredRecords = filteredRecords.Where(r => r.Timestamp >= query.StartTime.Value);
        
        if (query.EndTime.HasValue)
            filteredRecords = filteredRecords.Where(r => r.Timestamp <= query.EndTime.Value);

        // Apply field filters
        foreach (var filter in query.Filters)
        {
            filteredRecords = filteredRecords.Where(r => 
                r.Fields.ContainsKey(filter.Key) && 
                r.Fields[filter.Key]?.ToString() == filter.Value);
        }

        var recordList = filteredRecords.ToList();

        // Calculate metric based on type
        switch (query.MetricId)
        {
            case "request-count":
                result.DataPoints = CalculateRequestCount(recordList, query);
                break;
            case "response-time":
                result.DataPoints = CalculateResponseTime(recordList, query);
                break;
            case "bytes-sent":
                result.DataPoints = CalculateBytesSent(recordList, query);
                break;
            case "error-rate":
                result.DataPoints = CalculateErrorRate(recordList, query);
                break;
        }

        return Task.FromResult(result);
    }

    private DataRecord ParseLogEntry(List<string> fields, string[] values)
    {
        var record = new DataRecord
        {
            ProviderId = ProviderId
        };

        for (int i = 0; i < Math.Min(fields.Count, values.Length); i++)
        {
            var field = fields[i];
            var value = values[i];

            if (field == "date" && i + 1 < fields.Count && fields[i + 1] == "time")
            {
                // Combine date and time
                if (DateTime.TryParse($"{values[i]} {values[i + 1]}", out var timestamp))
                {
                    record.Timestamp = timestamp;
                }
            }
            else if (field == "time-taken" && int.TryParse(value, out var timeTaken))
            {
                record.Fields[field] = timeTaken;
            }
            else if (field == "sc-bytes" && int.TryParse(value, out var bytes))
            {
                record.Fields[field] = bytes;
            }
            else if (field == "sc-status" && int.TryParse(value, out var status))
            {
                record.Fields[field] = status;
            }
            else
            {
                record.Fields[field] = value;
            }
        }

        return record;
    }

    private List<MetricDataPoint> CalculateRequestCount(List<DataRecord> records, MetricQuery query)
    {
        if (string.IsNullOrEmpty(query.GroupBy))
        {
            return new List<MetricDataPoint>
            {
                new MetricDataPoint
                {
                    Timestamp = DateTime.UtcNow,
                    Value = records.Count
                }
            };
        }

        var grouped = records
            .GroupBy(r => r.Fields.GetValueOrDefault(query.GroupBy, "unknown"))
            .Select(g => new MetricDataPoint
            {
                Timestamp = DateTime.UtcNow,
                Value = g.Count(),
                Labels = new Dictionary<string, string> { { query.GroupBy, g.Key?.ToString() ?? "unknown" } }
            })
            .ToList();

        return grouped;
    }

    private List<MetricDataPoint> CalculateResponseTime(List<DataRecord> records, MetricQuery query)
    {
        var timeTakenRecords = records
            .Where(r => r.Fields.ContainsKey("time-taken"))
            .Select(r => Convert.ToDouble(r.Fields["time-taken"]))
            .ToList();

        if (!timeTakenRecords.Any())
            return new List<MetricDataPoint>();

        var value = query.Aggregation switch
        {
            "avg" => timeTakenRecords.Average(),
            "min" => timeTakenRecords.Min(),
            "max" => timeTakenRecords.Max(),
            "p95" => CalculatePercentile(timeTakenRecords, 95),
            "p99" => CalculatePercentile(timeTakenRecords, 99),
            _ => timeTakenRecords.Average()
        };

        return new List<MetricDataPoint>
        {
            new MetricDataPoint
            {
                Timestamp = DateTime.UtcNow,
                Value = value
            }
        };
    }

    private List<MetricDataPoint> CalculateBytesSent(List<DataRecord> records, MetricQuery query)
    {
        var bytesRecords = records
            .Where(r => r.Fields.ContainsKey("sc-bytes"))
            .Select(r => Convert.ToDouble(r.Fields["sc-bytes"]))
            .ToList();

        if (!bytesRecords.Any())
            return new List<MetricDataPoint>();

        var value = query.Aggregation == "avg" ? bytesRecords.Average() : bytesRecords.Sum();

        return new List<MetricDataPoint>
        {
            new MetricDataPoint
            {
                Timestamp = DateTime.UtcNow,
                Value = value
            }
        };
    }

    private List<MetricDataPoint> CalculateErrorRate(List<DataRecord> records, MetricQuery query)
    {
        var totalRequests = records.Count;
        var errorRequests = records.Count(r => 
            r.Fields.ContainsKey("sc-status") && 
            Convert.ToInt32(r.Fields["sc-status"]) >= 400);

        var errorRate = totalRequests > 0 ? (double)errorRequests / totalRequests * 100 : 0;

        return new List<MetricDataPoint>
        {
            new MetricDataPoint
            {
                Timestamp = DateTime.UtcNow,
                Value = errorRate
            }
        };
    }

    private double CalculatePercentile(List<double> values, int percentile)
    {
        var sorted = values.OrderBy(v => v).ToList();
        var index = (int)Math.Ceiling(sorted.Count * percentile / 100.0) - 1;
        return sorted[Math.Max(0, Math.Min(index, sorted.Count - 1))];
    }
}
