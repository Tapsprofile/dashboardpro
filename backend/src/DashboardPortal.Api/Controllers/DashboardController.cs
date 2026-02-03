using DashboardPortal.Application.Abstractions.Analytics;
using DashboardPortal.Domain.Configurations;
using DashboardPortal.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardPortal.Api.Controllers;

[ApiController]
[Route("api/dashboards")]
[Authorize(Roles = Roles.Admin + "," + Roles.User)]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardAggregationService _aggregationService;

    public DashboardController(IDashboardAggregationService aggregationService)
    {
        _aggregationService = aggregationService;
    }

    [HttpPost("execute")]
    public async Task<ActionResult<IReadOnlyList<WidgetResult>>> ExecuteAsync(
        [FromBody] DashboardConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var results = await _aggregationService.RunAsync(configuration, cancellationToken);
        return Ok(results);
    }
}
