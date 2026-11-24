using LabPulse.DAL.DataContext.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LabPulse.DAL;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var scopedProvider = scope.ServiceProvider;

        var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = scopedProvider.GetRequiredService<UserManager<User>>();
        var configuration = scopedProvider.GetRequiredService<IConfiguration>();

        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create '{adminRole}' role: {errors}");
            }
        }

        var adminUserName = configuration["Seed:AdminUserName"];
        if (string.IsNullOrWhiteSpace(adminUserName))
        {
            adminUserName = "admin";
        }

        var existingUser = await userManager.FindByNameAsync(adminUserName);
        if (existingUser is null)
        {
            var adminPassword = configuration["Seed:AdminPassword"];
            if (string.IsNullOrWhiteSpace(adminPassword))
            {
                adminPassword = "MyAdmin#2026pass";
            }

            var adminUser = new User
            {
                UserName = adminUserName,
                Email = adminUserName.Contains('@') ? adminUserName : null,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create admin user '{adminUserName}': {errors}");
            }

            var roleAssignResult = await userManager.AddToRoleAsync(adminUser, adminRole);
            if (!roleAssignResult.Succeeded)
            {
                var errors = string.Join(", ", roleAssignResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to assign role '{adminRole}' to user '{adminUserName}': {errors}");
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(existingUser, adminRole))
            {
                await userManager.AddToRoleAsync(existingUser, adminRole);
            }
        }
    }
}
