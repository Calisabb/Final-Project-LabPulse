using LabPulse.BLL.DTOs.Incidents;
using LabPulse.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LabPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IncidentsController : ControllerBase
{
    private readonly IIncidentService _incidents;

    public IncidentsController(IIncidentService incidents)
    {
        _incidents = incidents;
    }
    [HttpGet("incidents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IncidentDto>>> GetAll([FromQuery] bool onlyOpen = false)
    { 
        return Ok(await _incidents.GetAllAsync(onlyOpen));
    }

    [HttpGet("services/{serviceId:int}/incidents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<IncidentDto>>> GetByService(int serviceId)
    {
        return Ok(await _incidents.GetByServiceAsync(serviceId));
    }
}