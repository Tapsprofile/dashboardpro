using System.Runtime.CompilerServices;
using DashboardPortal.Domain.Entities;

namespace DashboardPortal.Infrastructure.Parsing;

public sealed class IisW3cLogParser
{
    private static readonly char[] Separator = [' '];

    public async IAsyncEnumerable<LogEntry> ParseAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, leaveOpen: true);
        string? line;
        string[]? fields = null;

        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (line.StartsWith("#Fields:", StringComparison.OrdinalIgnoreCase))
            {
                fields = line["#Fields:".Length..]
                    .Trim()
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                continue;
            }

            if (line.StartsWith('#'))
            {
                continue;
            }

            if (fields == null || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var values = line.Split(Separator, StringSplitOptions.None);
            if (values.Length != fields.Length)
            {
                continue;
            }

            var entry = new LogEntry();
            for (var index = 0; index < fields.Length; index++)
            {
                MapField(entry, fields[index], values[index]);
            }

            yield return entry;
        }
    }

    private static void MapField(LogEntry entry, string field, string rawValue)
    {
        if (rawValue == "-")
        {
            rawValue = string.Empty;
        }

        switch (field)
        {
            case "date":
                entry.Timestamp = ParseDateTime(rawValue, entry.Timestamp, isDate: true);
                return;
            case "time":
                entry.Timestamp = ParseDateTime(rawValue, entry.Timestamp, isDate: false);
                return;
            case "c-ip":
                entry.ClientIp = rawValue;
                return;
            case "s-ip":
                entry.ServerIp = rawValue;
                return;
            case "s-port":
                entry.ServerPort = rawValue;
                return;
            case "s-sitename":
                entry.SiteName = rawValue;
                return;
            case "cs-host":
                entry.Host = rawValue;
                return;
            case "cs-method":
                entry.Method = rawValue;
                return;
            case "cs-uri-stem":
                entry.UriStem = rawValue;
                return;
            case "cs-uri-query":
                entry.UriQuery = rawValue;
                return;
            case "cs-username":
                entry.Username = rawValue;
                return;
            case "cs(User-Agent)":
                entry.UserAgent = rawValue;
                return;
            case "cs(Referer)":
                entry.Referrer = rawValue;
                return;
            case "sc-status":
                entry.StatusCode = ParseInt(rawValue);
                return;
            case "sc-substatus":
                entry.SubStatusCode = ParseInt(rawValue);
                return;
            case "sc-win32-status":
                entry.Win32Status = ParseInt(rawValue);
                return;
            case "time-taken":
                entry.TimeTakenMs = ParseLong(rawValue);
                return;
            case "sc-bytes":
                entry.BytesSent = ParseLong(rawValue);
                return;
            case "cs-bytes":
                entry.BytesReceived = ParseLong(rawValue);
                return;
            default:
                entry.CustomProperties[field] = rawValue;
                return;
        }
    }

    private static DateTimeOffset ParseDateTime(string rawValue, DateTimeOffset current, bool isDate)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return current;
        }

        if (isDate)
        {
            if (DateOnly.TryParse(rawValue, out var date))
            {
                return new DateTimeOffset(
                    date.Year,
                    date.Month,
                    date.Day,
                    current.Hour,
                    current.Minute,
                    current.Second,
                    TimeSpan.Zero);
            }

            return current;
        }

        if (TimeOnly.TryParse(rawValue, out var time))
        {
            return new DateTimeOffset(
                current.Year,
                current.Month,
                current.Day,
                time.Hour,
                time.Minute,
                time.Second,
                TimeSpan.Zero);
        }

        return current;
    }

    private static int? ParseInt(string rawValue)
    {
        return int.TryParse(rawValue, out var value) ? value : null;
    }

    private static long? ParseLong(string rawValue)
    {
        return long.TryParse(rawValue, out var value) ? value : null;
    }
}
