using LabPulse.BLL.DTOs.Backup;
using LabPulse.BLL.Services.Contracts;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.Repositories.Contracts;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace LabPulse.BLL.Services.Implementations;

internal class BackupService : IBackupService
{
    private const int CurrentVersion = 1;
    private const int MaxServicesInFile = 500;
    private static readonly  Regex ColorRegex = new("^#[0-9A-Fa-f]{6}$");

    private readonly ICategoryRepositoryAsync _categories;
    private readonly IServiceRepositoryAsync _services;
    private readonly IRunbookRepositoryAsync _runbooks;
    public BackupService(
        ICategoryRepositoryAsync categories,
        IServiceRepositoryAsync services,
        IRunbookRepositoryAsync runbooks)
    {
        _categories = categories;
        _services = services;
        _runbooks = runbooks;
    }


    public async Task<BackupDto> ExportAsync()
    {
        var categories = await _categories.GetAllAsync();
        var services = await _services.GetAllAsync();
        var runbooks = await _runbooks.GetAllAsync();

        var categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);
        var runbooksByService = runbooks.ToLookup(r => r.ServiceId);

        return new BackupDto
        {
            Version = CurrentVersion,
            Categories = categories
            .OrderBy(c=>c.Name)
            .Select(c => new BackupCategoryDto
            {
                Name = c.Name,
                Color = c.Color
            }).ToList(),
            Services = services
            .OrderBy(s => s.Name)
            .Select(s=>new BackupServiceDto
            {
                Name = s.Name,
                Host = s.Host,
                Port = s.Port,
                CheckType = s.CheckType,
                UseHttps = s.UseHttps,
                HealthPath = s.HealthPath,
                TimeoutMs = s.TimeoutMs,
                IsMonitoringEnabled = s.IsMonitoringEnabled,
                Description = s.Description,
                Icon = s.Icon,
                CategoryName = categoryNames[s.CategoryId],
                Runbooks = runbooksByService[s.Id]
                .OrderBy(r=>r.Id)
                .Select(r => new BackupRunbookDto
                {
                    Title = r.Title,
                    ShellType = r.ShellType,
                    CommandText = r.CommandText,
                    Notes = r.Notes
                })
                .ToList()
            })
            .ToList()
        };
    }

    public async Task<ImportResultDto> ImportAsync(BackupDto backup)
    {
        if (backup == null)
            throw new ArgumentNullException("Backup file is empty.");
        backup.Categories ??= new List<BackupCategoryDto>();
        backup.Services ??= new List<BackupServiceDto>();
        Validate(backup);

        var now = DateTime.UtcNow;
        var result = new ImportResultDto();

        var existingCategories = await _categories.GetAllAsync();
        var categoryIds = existingCategories
            .ToDictionary(c => c.Name, c => c.Id, StringComparer.OrdinalIgnoreCase);

        var newCategories = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

        foreach (var dto in backup.Categories)
        {
            var name = dto.Name.Trim();
            if (categoryIds.ContainsKey(name) || newCategories.ContainsKey(name))
            { 
                result.CategoriesSkipped++;
                continue;
            }
            var category = new Category
            {
                Name = name,
                Color = dto.Color,
                CreatedAt = now
            };
            await _categories.AddAsync(category);
            newCategories[name] = category;
            result.CategoriesAdded++;
        }

        var existingServices = await _services.GetAllAsync();
        var knownEndpoints = existingServices
            .Select(s => Endpoint(s.Host, s.Port))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var dto in backup.Services)
        {
            if (!knownEndpoints.Add(Endpoint(dto.Host, dto.Port)))
            {
                result.ServicesSkipped++;
                continue;
            }

            var service = new Service
            {
                Name = dto.Name.Trim(),
                Host = dto.Host.Trim(),
                Port = dto.Port,
                CheckType = dto.CheckType,
                UseHttps = dto.UseHttps,
                HealthPath = dto.HealthPath,
                TimeoutMs = dto.TimeoutMs,
                IsMonitoringEnabled = dto.IsMonitoringEnabled,
                Description = dto.Description,
                Icon = dto.Icon,
                CreatedAt = now
            };

            var categoryName = dto.CategoryName.Trim();
            if (newCategories.TryGetValue(categoryName, out var newCategory))
                service.Category = newCategory;
            else if (categoryIds.TryGetValue(categoryName, out var categoryId))
                service.CategoryId = categoryId;
            else
                throw new ArgumentException(
                    $"Category '{categoryName}' of service '{dto.Name}' is not in the file or the database.");

            foreach (var r in dto.Runbooks)
            {
                service.Runbooks.Add(new Runbook
                {
                    Title = r.Title.Trim(),
                    ShellType = r.ShellType,
                    CommandText = r.CommandText,
                    Notes = r.Notes,
                    CreatedAt = now
                });
                result.RunbooksAdded++;
            }

            await _services.AddAsync(service);
            result.ServicesAdded++;
        }

        await _services.SaveChangesAsync();
        return result;

    }
    private static string Endpoint(string host, int port) => $"{host.Trim()}:{port}";
    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new ArgumentException(message);
    }
    private static void Validate(BackupDto backup)
    {
        Require(backup.Version == CurrentVersion,
            $"Unsupported backup version {backup.Version}. Expected {CurrentVersion}.");
        Require(backup.Services.Count <= MaxServicesInFile,
            $"Too many services in file (max {MaxServicesInFile}).");

        foreach (var c in backup.Categories)
        {
            Require(!string.IsNullOrWhiteSpace(c.Name) && c.Name.Trim().Length <= 50,
                "Category name is required (max 50 characters).");
            Require(string.IsNullOrEmpty(c.Color) || ColorRegex.IsMatch(c.Color),
                $"Category '{c.Name}': color must look like #RRGGBB.");
        }

        foreach (var s in backup.Services)
        {
            s.Runbooks ??= new List<BackupRunbookDto>();

            Require(!string.IsNullOrWhiteSpace(s.Name) && s.Name.Trim().Length <= 100,
                "Service name is required (max 100 characters).");
            Require(!string.IsNullOrWhiteSpace(s.Host) && s.Host.Trim().Length <= 255,
                $"Service '{s.Name}': host is required (max 255 characters).");
            Require(s.Port is >= 1 and <= 65535,
                $"Service '{s.Name}': port must be 1-65535.");
            Require(Enum.IsDefined(s.CheckType),
                $"Service '{s.Name}': unknown check type.");
            Require(s.TimeoutMs is > 0 and <= 60000,
                $"Service '{s.Name}': timeout must be 1-60000 ms.");
            Require(!string.IsNullOrWhiteSpace(s.CategoryName),
                $"Service '{s.Name}': category is required.");

            foreach (var r in s.Runbooks)
            {
                Require(!string.IsNullOrWhiteSpace(r.Title) && r.Title.Trim().Length <= 150,
                    $"Service '{s.Name}': runbook title is required (max 150 characters).");
                Require(!string.IsNullOrWhiteSpace(r.CommandText),
                    $"Service '{s.Name}': runbook '{r.Title}' has no command text.");
                Require(Enum.IsDefined(r.ShellType),
                    $"Service '{s.Name}': runbook '{r.Title}' has unknown shell type.");
            }
        }
    }
}