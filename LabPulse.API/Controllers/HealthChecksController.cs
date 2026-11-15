using LabPulse.BLL.DTOs.HealthChecks;
using LabPulse.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LabPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthChecksController : ControllerBase
{
    private readonly IHealthCheckService _healthChecks;

    public HealthChecksController(IHealthCheckService healthCheckService)
    {
        _healthChecks = healthCheckService;
    }

    [HttpPost("services/{serviceId:int}/check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceCheckResultDto>> CheckHealth(int serviceId, CancellationToken ct)
    {
        return Ok(await _healthChecks.CheckServiceAsync(serviceId, ct));
    }

    [HttpPost("checks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<BulkCheckResultDto>> CheckAll(CancellationToken ct)
    {
        return Ok(await _healthChecks.CheckAllAsync(ct));
    }
}