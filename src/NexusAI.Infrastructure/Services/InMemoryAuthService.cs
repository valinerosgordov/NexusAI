using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Infrastructure.Services;

public sealed class InMemoryAuthService : IAuthService
{
    private readonly ConcurrentDictionary<UserId, User> _users = new();
    private readonly ConcurrentDictionary<string, UserId> _nameIndex = new(StringComparer.OrdinalIgnoreCase);

    public Task<Result<User>> RegisterAsync(string name, string password, CancellationToken ct = default)
    {
        if (_nameIndex.ContainsKey(name))
            return Task.FromResult(Result<User>.Failure("User already exists"));

        var userId = new UserId(Guid.NewGuid());
        var user = new User
        {
            Id = userId,
            Name = name,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        _users[userId] = user;
        _nameIndex[name] = userId;
        return Task.FromResult(Result<User>.Success(user));
    }

    public Task<Result<User>> LoginAsync(string name, string password, CancellationToken ct = default)
    {
        if (!_nameIndex.TryGetValue(name, out var userId) || !_users.TryGetValue(userId, out var user))
            return Task.FromResult(Result<User>.Failure("Invalid credentials"));

        return Task.FromResult(
            string.Equals(user.PasswordHash, HashPassword(password), StringComparison.Ordinal)
                ? Result<User>.Success(user)
                : Result<User>.Failure("Invalid credentials"));
    }

    public Task<Result<User>> GetUserByIdAsync(UserId id, CancellationToken ct = default) =>
        Task.FromResult(_users.TryGetValue(id, out var user)
            ? Result<User>.Success(user)
            : Result<User>.Failure("User not found"));

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
