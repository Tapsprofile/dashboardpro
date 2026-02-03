using DashboardPro.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DashboardPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataController : ControllerBase
{
    private readonly IEnumerable<IDataProvider> _dataProviders;
    private readonly ILogger<DataController> _logger;

    public DataController(IEnumerable<IDataProvider> dataProviders, ILogger<DataController> logger)
    {
        _dataProviders = dataProviders;
        _logger = logger;
    }

    /// <summary>
    /// Upload and parse data file
    /// </summary>
    [HttpPost("providers/{providerId}/upload")]
    public async Task<ActionResult> UploadData(string providerId, IFormFile file)
    {
        var provider = _dataProviders.FirstOrDefault(p => p.ProviderId == providerId);
        if (provider == null)
        {
            return NotFound($"Provider '{providerId}' not found");
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        try
        {
            using var stream = file.OpenReadStream();
            var records = await provider.ParseDataAsync(stream);
            
            return Ok(new
            {
                recordCount = records.Count(),
                message = "Data parsed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing data file");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Bulk upload and parse multiple data files
    /// </summary>
    [HttpPost("providers/{providerId}/bulk-upload")]
    public async Task<ActionResult> BulkUploadData(string providerId, List<IFormFile> files)
    {
        var provider = _dataProviders.FirstOrDefault(p => p.ProviderId == providerId);
        if (provider == null)
        {
            return NotFound($"Provider '{providerId}' not found");
        }

        if (files == null || !files.Any())
        {
            return BadRequest("No files uploaded");
        }

        try
        {
            var streams = files.Select(f => f.OpenReadStream());
            var records = await provider.BulkParseAsync(streams);
            
            return Ok(new
            {
                fileCount = files.Count,
                recordCount = records.Count(),
                message = "Data parsed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing data files");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
