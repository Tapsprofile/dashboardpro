using DashboardPro.Core.Models.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DashboardPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardsController : ControllerBase
{
    private static readonly List<DashboardConfig> _dashboards = new();
    private readonly ILogger<DashboardsController> _logger;

    public DashboardsController(ILogger<DashboardsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all dashboards
    /// </summary>
    [HttpGet]
    public ActionResult<IEnumerable<DashboardConfig>> GetAll()
    {
        return Ok(_dashboards);
    }

    /// <summary>
    /// Get dashboard by ID
    /// </summary>
    [HttpGet("{id}")]
    public ActionResult<DashboardConfig> GetById(string id)
    {
        var dashboard = _dashboards.FirstOrDefault(d => d.Id == id);
        if (dashboard == null)
        {
            return NotFound();
        }
        return Ok(dashboard);
    }

    /// <summary>
    /// Create new dashboard
    /// </summary>
    [HttpPost]
    public ActionResult<DashboardConfig> Create([FromBody] DashboardConfig dashboard)
    {
        dashboard.Id = Guid.NewGuid().ToString();
        dashboard.CreatedAt = DateTime.UtcNow;
        dashboard.UpdatedAt = DateTime.UtcNow;
        
        _dashboards.Add(dashboard);
        
        return CreatedAtAction(nameof(GetById), new { id = dashboard.Id }, dashboard);
    }

    /// <summary>
    /// Update existing dashboard
    /// </summary>
    [HttpPut("{id}")]
    public ActionResult<DashboardConfig> Update(string id, [FromBody] DashboardConfig dashboard)
    {
        var existing = _dashboards.FirstOrDefault(d => d.Id == id);
        if (existing == null)
        {
            return NotFound();
        }

        dashboard.Id = id;
        dashboard.CreatedAt = existing.CreatedAt;
        dashboard.UpdatedAt = DateTime.UtcNow;
        
        _dashboards.Remove(existing);
        _dashboards.Add(dashboard);
        
        return Ok(dashboard);
    }

    /// <summary>
    /// Delete dashboard
    /// </summary>
    [HttpDelete("{id}")]
    public ActionResult Delete(string id)
    {
        var dashboard = _dashboards.FirstOrDefault(d => d.Id == id);
        if (dashboard == null)
        {
            return NotFound();
        }

        _dashboards.Remove(dashboard);
        return NoContent();
    }
}
