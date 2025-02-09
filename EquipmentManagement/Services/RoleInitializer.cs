using EquipmentManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace EquipmentManagement.Helpers
{
    public static class RoleInitializer
    {
        public static async Task InitializeRoles(IServiceProvider serviceProvider, UserManager<User> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            var defaultAdminUser = await userManager.FindByEmailAsync("admin@admin.com");
            if (defaultAdminUser == null)
            {
                defaultAdminUser = new User { UserName = "admin", Email = "admin@admin.com" };
                await userManager.CreateAsync(defaultAdminUser, "Admin@123");
            }

            if (!await userManager.IsInRoleAsync(defaultAdminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(defaultAdminUser, "Admin");
            }
        }
    }
}
