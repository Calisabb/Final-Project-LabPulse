using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LabPulse.API.Controllers;

[ApiController]
[Route("api/services/{serviceId:int}")]
public class HealthLogsController : ControllerBase
{
    private readonly IHealthLogService _logs;

    public HealthLogsController(IHealthLogService logs)
    {
        _logs = logs;
    }

    [HttpGet("logs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HealthLogDto>>> GetLast(
        int serviceId, [FromQuery] int count = 50)
    {
        return Ok(await _logs.GetLastAsync(serviceId, count));
    }

    [HttpGet("logs/period")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<HealthLogDto>>> GetByPeriod(
        int serviceId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        return Ok(await _logs.GetByPeriodAsync(serviceId, from.ToUniversalTime(), to.ToUniversalTime()));
    }

    [HttpGet("uptime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UptimeDayDto>>> GetUptime(
        int serviceId, [FromQuery] int days = 30)
    {
        return Ok(await _logs.GetUptimeAsync(serviceId, days));
    }
}
