using Microsoft.AspNetCore.Identity;

namespace test.Services
{
    /// <summary>
    /// Provides functionality to initialize roles in the application.
    /// </summary>
    public static class RoleInitializer
    {
        /// <summary>
        /// Initializes predefined roles in the application if they don't already exist.
        /// </summary>
        /// <param name="roleManager">The role manager used to create and check roles.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "SubtitleCreator", "ContentCreator", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}