using Microsoft.EntityFrameworkCore;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Entities;
using NexusAI.Infrastructure.Persistence;
using System.Security.Cryptography;
using System.Text;

namespace NexusAI.Infrastructure.Services;

public sealed class AuthService(AppDbContext context) : IAuthService
{
    public async Task<Result<User>> RegisterAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Result<User>.Failure("Username cannot be empty");

        if (string.IsNullOrWhiteSpace(password))
            return Result<User>.Failure("Password cannot be empty");

        if (password.Length < 6)
            return Result<User>.Failure("Password must be at least 6 characters");

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct)
            .ConfigureAwait(false);

        if (existingUser is not null)
            return Result<User>.Failure($"User '{username}' already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct).ConfigureAwait(false);

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Result<User>.Failure("Username cannot be empty");

        if (string.IsNullOrWhiteSpace(password))
            return Result<User>.Failure("Password cannot be empty");

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct)
            .ConfigureAwait(false);

        if (user is null)
            return Result<User>.Failure("Invalid credentials");

        var passwordHash = HashPassword(password);
        
        if (user.PasswordHash != passwordHash)
            return Result<User>.Failure("Invalid credentials");

        return Result<User>.Success(user);
    }

    public async Task<Result<User>> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            .ConfigureAwait(false);

        return user is not null
            ? Result<User>.Success(user)
            : Result<User>.Failure("User not found");
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
