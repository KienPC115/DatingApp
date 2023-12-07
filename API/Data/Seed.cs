using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    // UserManager which provides us with a user manager service/ apis for managing user in a persistence store
    // RoleManager which provides us with a role manager service/ apis for managing role in a persistence store
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

        // create a 3 roles for application
        var roles = new List<AppRole>
        {
            new AppRole{ Name = "Member"},
            new AppRole{ Name = "Admin"},
            new AppRole{ Name = "Moderator"},
        };

        // we need add roles first then user can have a role for themself
        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {
            // using var hmac = new HMACSHA512();

            user.UserName = user.UserName.ToLower();
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            // user.PasswordSalt = hmac.Key;

            // nothing happen in database
            // just adding it to entity framework tracking -> old version

            // it like context.User.Add() -> but this is the service for only User -> will be stored immediately into database
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }

        var admin = new AppUser
        {
            UserName = "admin"
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        // AddToRolesAsync() can add a user to multiple roles
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
    }
}
