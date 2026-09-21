using Microsoft.AspNetCore.Identity;

namespace LifeSure.Data;

public static class AdminAccountSeeder
{
    public static async Task SeedAsync(
        IServiceProvider services,
        IConfiguration configuration)
    {
        var email = configuration["SeedAdmin:Email"]?.Trim();
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "SeedAdmin:Email ve SeedAdmin:Password ayarlarını doldurunuz.");
        }

        var userManager =
            services.GetRequiredService<UserManager<IdentityUser>>();

        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();

        const string roleName = "Admin";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var roleResult = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            EnsureSucceeded(roleResult);
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                LockoutEnabled = true
            };

            var createResult = await userManager.CreateAsync(
                user,
                password);

            EnsureSucceeded(createResult);
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            var roleResult = await userManager.AddToRoleAsync(
                user,
                roleName);

            EnsureSucceeded(roleResult);
        }
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join(
            " | ",
            result.Errors.Select(x => x.Description));

        throw new InvalidOperationException(errors);
    }
}