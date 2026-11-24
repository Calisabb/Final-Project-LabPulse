using LabPulse.DAL.DataContext;
using LabPulse.DAL.DataContext.Entities;
using LabPulse.DAL.DataContext.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LabPulse.DAL;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LabPulseDbContext>();
        var logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("DataSeeder");

        try
        {
            // 1. Ensure pending migrations are applied
            await context.Database.MigrateAsync();

            // 2. Ensure Categories exist
            var categories = await context.Categories.ToListAsync();

            Category GetOrCreateCategory(string name, string color)
            {
                var cat = categories.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (cat == null)
                {
                    cat = new Category { Name = name, Color = color, CreatedAt = DateTime.UtcNow };
                    context.Categories.Add(cat);
                    categories.Add(cat);
                }
                return cat;
            }

            var netCat = GetOrCreateCategory("Network Gateways", "#00f2ff");
            var dbCat = GetOrCreateCategory("Databases", "#10B981");
            var devCat = GetOrCreateCategory("Dev Environment", "#3B82F6");
            var mediaCat = GetOrCreateCategory("Media", "#F59E0B");

            await context.SaveChangesAsync();

            // 3. Ensure Services & Runbooks exist
            var existingServiceNames = await context.Services.Select(s => s.Name).ToListAsync();

            var seedServices = new List<(Service Service, List<Runbook> Runbooks)>
            {
                (
                    new Service
                    {
                        Name = "Cloudflare DNS Gateway",
                        Description = "Primary upstream public DNS resolver (TCP 3-way handshake verification).",
                        Icon = "globe",
                        Host = "1.1.1.1",
                        Port = 53,
                        UseHttps = false,
                        CheckType = CheckType.Tcp,
                        HealthPath = null,
                        TimeoutMs = 3000,
                        IsMonitoringEnabled = true,
                        CategoryId = netCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Flush DNS resolver cache", ShellType = ShellType.PowerShell, CommandText = "ipconfig /flushdns", Notes = "Flushes local OS DNS resolver buffer." },
                        new Runbook { Title = "Probe upstream DNS resolution", ShellType = ShellType.PowerShell, CommandText = "Resolve-DnsName -Server 1.1.1.1 google.com", Notes = "Runs direct A-record query against 1.1.1.1." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "Google Public DNS",
                        Description = "Secondary recursive public DNS resolver fallback gateway.",
                        Icon = "router",
                        Host = "8.8.8.8",
                        Port = 53,
                        UseHttps = false,
                        CheckType = CheckType.Tcp,
                        HealthPath = null,
                        TimeoutMs = 3000,
                        IsMonitoringEnabled = true,
                        CategoryId = netCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Test fallback DNS socket", ShellType = ShellType.PowerShell, CommandText = "Test-NetConnection -ComputerName 8.8.8.8 -Port 53", Notes = "Validates SYN/ACK response on port 53." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "LabPulse Telemetry API",
                        Description = "Local REST API telemetry engine & continuous background health probe worker.",
                        Icon = "terminal",
                        Host = "localhost",
                        Port = 5046,
                        UseHttps = false,
                        CheckType = CheckType.Http,
                        HealthPath = "/openapi/v1.json",
                        TimeoutMs = 2500,
                        IsMonitoringEnabled = true,
                        CategoryId = devCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Restart LabPulse backend API", ShellType = ShellType.PowerShell, CommandText = "dotnet run --project LabPulse.API", Notes = "Starts backend server with Swagger and OpenAPI spec." },
                        new Runbook { Title = "Inspect API OpenAPI contract", ShellType = ShellType.PowerShell, CommandText = "curl.exe -s http://localhost:5046/openapi/v1.json", Notes = "Fetches raw OpenAPI v3 JSON document." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "PostgreSQL Telemetry DB",
                        Description = "Relational time-series metrics, health logs and audit ledger store.",
                        Icon = "database",
                        Host = "127.0.0.1",
                        Port = 5432,
                        UseHttps = false,
                        CheckType = CheckType.Tcp,
                        HealthPath = null,
                        TimeoutMs = 3000,
                        IsMonitoringEnabled = true,
                        CategoryId = dbCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Restart PostgreSQL container", ShellType = ShellType.Docker, CommandText = "docker restart homelab_postgres", Notes = "Restarts database engine and reconnects pool." },
                        new Runbook { Title = "Check PostgreSQL container logs", ShellType = ShellType.Docker, CommandText = "docker logs --tail 50 homelab_postgres", Notes = "Inspects last 50 error entries in log." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "Redis Cache Broker",
                        Description = "Ephemeral latency telemetry buffer & distributed lock coordinator.",
                        Icon = "cpu",
                        Host = "127.0.0.1",
                        Port = 6379,
                        UseHttps = false,
                        CheckType = CheckType.Tcp,
                        HealthPath = null,
                        TimeoutMs = 2000,
                        IsMonitoringEnabled = true,
                        CategoryId = dbCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Ping Redis cache broker", ShellType = ShellType.Docker, CommandText = "docker exec -it homelab_redis redis-cli ping", Notes = "Verifies PONG reply from Redis daemon." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "Nginx Ingress Edge",
                        Description = "External reverse proxy with SSL/TLS edge termination and rate limiting.",
                        Icon = "server",
                        Host = "google.com",
                        Port = 443,
                        UseHttps = true,
                        CheckType = CheckType.Http,
                        HealthPath = "/",
                        TimeoutMs = 4000,
                        IsMonitoringEnabled = true,
                        CategoryId = netCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Reload Nginx configurations", ShellType = ShellType.Bash, CommandText = "nginx -s reload", Notes = "Performs zero-downtime hot reload." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "Jellyfin Media Server",
                        Description = "Homelab video transcoding, audio streaming and metadata provider hub.",
                        Icon = "film",
                        Host = "192.168.1.150",
                        Port = 8096,
                        UseHttps = false,
                        CheckType = CheckType.Http,
                        HealthPath = "/health",
                        TimeoutMs = 3000,
                        IsMonitoringEnabled = true,
                        CategoryId = mediaCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Restart Jellyfin media container", ShellType = ShellType.Docker, CommandText = "docker restart jellyfin", Notes = "Restarts media worker." }
                    }
                ),
                (
                    new Service
                    {
                        Name = "TrueNAS Storage Pool",
                        Description = "ZFS mirrored RAID array with NFS/SMB shared storage exports.",
                        Icon = "server",
                        Host = "192.168.1.200",
                        Port = 445,
                        UseHttps = false,
                        CheckType = CheckType.Tcp,
                        HealthPath = null,
                        TimeoutMs = 3000,
                        IsMonitoringEnabled = true,
                        CategoryId = dbCat.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new List<Runbook>
                    {
                        new Runbook { Title = "Check ZFS storage pool health", ShellType = ShellType.Bash, CommandText = "zpool status -x", Notes = "Reports pool degradation or scrub errors." }
                    }
                )
            };

            foreach (var (service, runbooks) in seedServices)
            {
                var existing = await context.Services.FirstOrDefaultAsync(s => s.Name.ToLower() == service.Name.ToLower());
                if (existing == null)
                {
                    context.Services.Add(service);
                    await context.SaveChangesAsync();

                    foreach (var rb in runbooks)
                    {
                        rb.ServiceId = service.Id;
                        rb.CreatedAt = DateTime.UtcNow;
                        context.Runbooks.Add(rb);
                    }
                    await context.SaveChangesAsync();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(existing.Description))
                    {
                        existing.Description = service.Description;
                    }
                    if (string.IsNullOrWhiteSpace(existing.Icon))
                    {
                        existing.Icon = service.Icon;
                    }
                    await context.SaveChangesAsync();

                    var hasRunbooks = await context.Runbooks.AnyAsync(r => r.ServiceId == existing.Id);
                    if (!hasRunbooks)
                    {
                        foreach (var rb in runbooks)
                        {
                            rb.ServiceId = existing.Id;
                            rb.CreatedAt = DateTime.UtcNow;
                            context.Runbooks.Add(rb);
                        }
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while seeding demo services and categories.");
        }
    }
}
