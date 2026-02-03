using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Auth;

public record LoginUserCommand(string Name, string Password);

public class LoginUserHandler(IAuthService authService)
{
    public async Task<Result<User>> HandleAsync(LoginUserCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<User>.Failure("Name is required");

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result<User>.Failure("Password is required");

        return await authService.LoginAsync(command.Name, command.Password, ct);
    }
}
