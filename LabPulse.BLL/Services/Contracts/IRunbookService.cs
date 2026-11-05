using LabPulse.BLL.DTOs.Runbooks;

namespace LabPulse.BLL.Services.Contracts;

public interface IRunbookService
{
    Task<List<RunbookDto>> GetByServiceAsync(int serviceId);
    Task<RunbookDto> GetByIdAsync(int id);
    Task<RunbookDto> CreateAsync(CreateRunbookDto dto);
    Task<RunbookDto> UpdateAsync(int id, UpdateRunbookDto dto);
    Task DeleteAsync(int id);
}