using LabPulse.BLL.DTOs.Backup;
using LabPulse.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LabPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BackupController : ControllerBase
{
    private readonly IBackupService _backup;

    public BackupController(IBackupService backup)
    {
        _backup = backup;
    }

    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BackupDto>> Export()
    { 
        var backup = await _backup.ExportAsync();

        var fileName = $"labpulse-backup-{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";
        
        return Ok(backup);
    }

    [HttpPost("import")]
    [RequestSizeLimit(5_000_000)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ImportResultDto>> Import([FromBody] BackupDto backup)
    { 
        return await _backup.ImportAsync(backup);
    }
}