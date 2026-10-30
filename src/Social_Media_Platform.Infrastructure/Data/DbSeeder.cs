using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data;

public sealed class DbSeeder(
    Social_Media_PlatformDbContext dbContext,
    IConfiguration configuration)
{
    private static readonly RoleName[] InitialRoles =
    [
        RoleName.Admin,
        RoleName.Moderator,
        RoleName.User
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var adminAccounts = new[]
        {
            ReadAdminAccount("1"),
            ReadAdminAccount("2")
        };

        if (adminAccounts[0].Email.Equals(adminAccounts[1].Email, StringComparison.OrdinalIgnoreCase) ||
            adminAccounts[0].Username.Equals(adminAccounts[1].Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The two seeded admin accounts must have distinct email addresses and usernames.");
        }

        var existingRoles = await dbContext.Roles
            .Select(role => role.Name)
            .ToHashSetAsync(cancellationToken);

        var missingRoles = InitialRoles
            .Where(roleName => !existingRoles.Contains(roleName))
            .Select(roleName => new Role { Name = roleName })
            .ToArray();

        if (missingRoles.Length > 0)
        {
            dbContext.Roles.AddRange(missingRoles);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var adminRole = await dbContext.Roles
            .SingleAsync(role => role.Name == RoleName.Admin, cancellationToken);

        foreach (var account in adminAccounts)
        {
            var matches = await dbContext.Users
                .IgnoreQueryFilters()
                .Where(user => user.Email == account.Email || user.Username == account.Username)
                .ToListAsync(cancellationToken);

            if (matches.Count > 0)
            {
                var alreadySeeded = matches.Count == 1 &&
                    matches[0].RoleId == adminRole.Id &&
                    matches[0].Email.Equals(account.Email, StringComparison.OrdinalIgnoreCase) &&
                    matches[0].Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase);

                if (!alreadySeeded)
                {
                    throw new InvalidOperationException(
                        "An existing user conflicts with the configured admin seed account. " +
                        "Use unique email addresses and usernames.");
                }

                continue;
            }

            dbContext.Users.Add(new User
            {
                RoleId = adminRole.Id,
                Email = account.Email,
                Username = account.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(account.Password),
                IsVerified = true,
                Status = UserStatus.Active
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private AdminSeedAccount ReadAdminAccount(string number)
    {
        var prefix = $"SeedUsers:Admin{number}";
        var email = RequiredSetting($"{prefix}:Email");
        var username = RequiredSetting($"{prefix}:Username");
        var password = RequiredSetting($"{prefix}:Password");

        if (email.Length > 256 || username.Length > 50)
        {
            throw new InvalidOperationException(
                $"Settings '{prefix}:Email' and '{prefix}:Username' exceed the supported length.");
        }

        if (System.Text.Encoding.UTF8.GetByteCount(password) > 72)
        {
            throw new InvalidOperationException(
                $"Setting '{prefix}:Password' exceeds the maximum supported password length.");
        }

        return new AdminSeedAccount(email, username, password);
    }

    private string RequiredSetting(string key)
    {
        var value = configuration[key]?.Trim();
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Required admin seed setting '{key}' is missing. Configure it in the ignored .env file " +
                "or in a secure environment-variable provider.");
        }

        return value;
    }

    private sealed record AdminSeedAccount(string Email, string Username, string Password);
}
