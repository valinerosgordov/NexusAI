using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.Interfaces;

public interface IAuthService
{
    Task<Result<User>> RegisterAsync(string name, string password, CancellationToken ct = default);
    Task<Result<User>> LoginAsync(string name, string password, CancellationToken ct = default);
    Task<Result<User>> GetUserByIdAsync(UserId id, CancellationToken ct = default);
}
