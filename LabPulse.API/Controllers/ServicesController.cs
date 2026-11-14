using LabPulse.BLL.DTOs.Services;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LabPulse.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServicesController : ControllerBase
{
    private readonly IServiceCatalogService _services;

    public ServicesController(IServiceCatalogService services)
    {
        _services = services;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ServiceDto>>> GetAll(
        [FromQuery] ServiceStatus? status,
        [FromQuery] int? categoryId,
        [FromQuery] string? search)
    {
        return Ok(await _services.GetAllAsync(status, categoryId, search));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceDto>> GetById(int id)
    {
        return Ok(await _services.GetByIdAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ServiceDto>> Create(CreateServiceDto dto)
    {
        var created = await _services.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ServiceDto>> Update(int id, UpdateServiceDto dto)
    {
        return Ok(await _services.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _services.DeleteAsync(id);
        return NoContent();
    }
}