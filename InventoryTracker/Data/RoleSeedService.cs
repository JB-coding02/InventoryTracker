using InventoryTracker.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryTracker.Data;

public static class RoleSeedService
{
    // Hidden admin credentials - only visible to developers in source code
    private const string AdminEmail = "admin@inventorytracker.local";
    private const string AdminPassword = "NoArchitects@2024";

    public static async Task SeedAdminAccountAsync(IServiceProvider serviceProvider)
    {
		using IServiceScope scope = serviceProvider.CreateScope();
        
        UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Check if admin account already exists by email (email is used for login)
        ApplicationUser? adminUser = await userManager.FindByEmailAsync(AdminEmail);
        if (adminUser == null)
        {
            // Create the admin account with email as username for consistency with the rest of the app
            ApplicationUser newAdminUser = new ApplicationUser
            {
                UserName = AdminEmail,
                Email = AdminEmail,
                EmailConfirmed = true,
                UserRole = UserRole.Admin
            };

            IdentityResult result = await userManager.CreateAsync(newAdminUser, AdminPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                Console.WriteLine($"[AdminSeedService] Failed to create admin account: {errors}");
                throw new InvalidOperationException($"Failed to create admin account: {errors}");
            }
            else
            {
                Console.WriteLine($"[AdminSeedService] Admin account '{AdminEmail}' created successfully.");
            }
        }
        else
        {
            // Ensure existing admin account has the correct role and email is confirmed
            bool needsUpdate = false;

            if (adminUser.UserRole != UserRole.Admin)
            {
                adminUser.UserRole = UserRole.Admin;
                needsUpdate = true;
            }

            if (!adminUser.EmailConfirmed)
            {
                adminUser.EmailConfirmed = true;
                needsUpdate = true;
            }

            if (needsUpdate)
            {
                await userManager.UpdateAsync(adminUser);
                Console.WriteLine($"[AdminSeedService] Admin account '{AdminEmail}' updated.");
            }
            else
            {
                Console.WriteLine($"[AdminSeedService] Admin account '{AdminEmail}' already exists and is properly configured.");
            }
        }
    }
}
