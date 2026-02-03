using DashboardPro.Core.Interfaces;
using DashboardPro.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace DashboardPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IEnumerable<IDataProvider> _dataProviders;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(IEnumerable<IDataProvider> dataProviders, ILogger<MetricsController> logger)
    {
        _dataProviders = dataProviders;
        _logger = logger;
    }

    /// <summary>
    /// Get all available data providers
    /// </summary>
    [HttpGet("providers")]
    public ActionResult<IEnumerable<object>> GetProviders()
    {
        var providers = _dataProviders.Select(p => new
        {
            p.ProviderId,
            p.ProviderName
        });

        return Ok(providers);
    }

    /// <summary>
    /// Get available metrics for a specific provider
    /// </summary>
    [HttpGet("providers/{providerId}/metrics")]
    public async Task<ActionResult<IEnumerable<MetricDefinition>>> GetMetrics(string providerId)
    {
        var provider = _dataProviders.FirstOrDefault(p => p.ProviderId == providerId);
        if (provider == null)
        {
            return NotFound($"Provider '{providerId}' not found");
        }

        var metrics = await provider.GetAvailableMetricsAsync();
        return Ok(metrics);
    }

    /// <summary>
    /// Query metric data
    /// </summary>
    [HttpPost("providers/{providerId}/query")]
    public async Task<ActionResult<MetricResult>> QueryMetric(string providerId, [FromBody] MetricQuery query)
    {
        var provider = _dataProviders.FirstOrDefault(p => p.ProviderId == providerId);
        if (provider == null)
        {
            return NotFound($"Provider '{providerId}' not found");
        }

        try
        {
            var result = await provider.GetMetricAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error querying metric {MetricId}", query.MetricId);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
