using LabPulse.BLL.DTOs.Runbooks;
using LabPulse.BLL.Exceptions;
using LabPulse.BLL.Mapping;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.Repositories.Contracts;

namespace LabPulse.BLL.Services.Implementations;

public class RunbookService : IRunbookService
{
    private readonly IRunbookRepositoryAsync _runbooks;
    private readonly IServiceRepositoryAsync _services;

    public RunbookService(IRunbookRepositoryAsync runbooks, IServiceRepositoryAsync services)
    {
        _runbooks = runbooks;
        _services = services;
    }

    public async Task<RunbookDto> CreateAsync(CreateRunbookDto dto)
    {
        var service = await _services.GetByIdAsync(dto.ServiceId);
        if (service == null)
        {
            throw new KeyNotFoundException($"Service with ID {dto.ServiceId} does not exist.");
        }
        var title = dto.Title.Trim();
        if (await _runbooks.ExistsAsync(r => r.Title == title && r.ServiceId == dto.ServiceId))
        {
            throw new ConflictException($"Runbook with title '{title}' already exists for service ID {dto.ServiceId}.");
        }
        var entity = dto.ToEntity();
        await _runbooks.AddAsync(entity);
        await _runbooks.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        if (!await _runbooks.Delete(id))
        {
            throw new KeyNotFoundException($"Runbook with ID {id} does not exist.");
        }
        await _runbooks.SaveChangesAsync();
    }

    public async Task<RunbookDto> GetByIdAsync(int id)
    {
        var runbook = await _runbooks.GetByIdAsync(id);
        if (runbook == null)
        {
            throw new KeyNotFoundException($"Runbook with ID {id} does not exist.");
        }
        return runbook.ToDto();
    }

    public async Task<List<RunbookDto>> GetByServiceAsync(int serviceId)
    {
        if (!await _services.ExistsAsync(s => s.Id == serviceId))
        {
            throw new KeyNotFoundException($"Service with ID {serviceId} does not exist.");
        }
        var runbooks = await _runbooks.GetAllByServiceIdAsync(serviceId);
        return runbooks.Select(r => r.ToDto()).ToList();
    }

    public async Task<RunbookDto> UpdateAsync(int id, UpdateRunbookDto dto)
    {
        var runbook = await _runbooks.GetByIdAsync(id);
        if (runbook == null)
        {
            throw new KeyNotFoundException($"Runbook with ID {id} does not exist.");
        }
        var title = dto.Title.Trim();
        if (await _runbooks.ExistsAsync(r => r.Title == title && r.ServiceId == runbook.ServiceId && r.Id != id))
        {
            throw new ConflictException($"Runbook with title '{title}' already exists for service ID {runbook.ServiceId}.");
        }
        dto.ApplyTo(runbook);
        await _runbooks.SaveChangesAsync();
        return runbook.ToDto();
    }
}