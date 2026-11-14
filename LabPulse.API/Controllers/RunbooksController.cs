using LabPulse.BLL.DTOs.Runbooks;
using LabPulse.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LabPulse.API.Controllers;

[ApiController]
[Route("api")]
public class RunbooksController : ControllerBase
{
    private readonly IRunbookService _runbooks;

    public RunbooksController(IRunbookService runbooks)
    {
        _runbooks = runbooks;
    }

    [HttpGet("services/{serviceId:int}/runbooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<RunbookDto>>> GetByService(int serviceId)
    {
        return Ok(await _runbooks.GetByServiceAsync(serviceId));
    }

    [HttpGet("runbooks/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RunbookDto>> GetById(int id)
    {
        return Ok(await _runbooks.GetByIdAsync(id));
    }

    [HttpPost("runbooks")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RunbookDto>> Create(CreateRunbookDto dto)
    {
        var created = await _runbooks.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("runbooks/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RunbookDto>> Update(int id, UpdateRunbookDto dto)
    {
        return Ok(await _runbooks.UpdateAsync(id, dto));
    }

    [HttpDelete("runbooks/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _runbooks.DeleteAsync(id);
        return NoContent();
    }
}