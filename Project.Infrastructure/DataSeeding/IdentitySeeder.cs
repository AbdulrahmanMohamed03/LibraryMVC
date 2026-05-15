using Microsoft.AspNetCore.Identity;
using Project.Core.Models;

namespace Project.Infrastructure.DataSeeding
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            string roleName = "Admin";
            string roleUser = "User";
            string roleLibrarian = "Librarian";
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            if (!await roleManager.RoleExistsAsync(roleUser))
            {
                await roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            if (!await roleManager.RoleExistsAsync(roleLibrarian))
            {
                await roleManager.CreateAsync(new IdentityRole(roleLibrarian));
            }
            string adminEmail = "admin123@gmail.com";
            string fullName = "System Admin";
            string nationalId = "30304041402753";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = fullName,
                    NationalId = nationalId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(user, "Admin123@");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}