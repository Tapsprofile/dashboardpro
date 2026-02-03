using DashboardPortal.Application.Abstractions.Imports;
using DashboardPortal.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardPortal.Api.Controllers;

[ApiController]
[Route("api/imports")]
[Authorize(Roles = Roles.Admin)]
public sealed class ImportController : ControllerBase
{
    private readonly IImportQueue _queue;

    public ImportController(IImportQueue queue)
    {
        _queue = queue;
    }

    [HttpPost("iis")]
    [RequestSizeLimit(200_000_000)]
    public async Task<ActionResult> UploadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"iis-import-{Guid.NewGuid():N}-{file.FileName}");

        await using (var target = System.IO.File.Create(tempPath))
        {
            await file.CopyToAsync(target, cancellationToken);
        }

        _queue.Enqueue(new ImportRequest(file.FileName, tempPath));
        return Accepted();
    }
}
