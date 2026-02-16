#pragma warning disable MA0048
using NexusAI.Application.Interfaces;
using NexusAI.Domain.Common;
using NexusAI.Domain.Models;

namespace NexusAI.Application.UseCases.Auth;

public record RegisterUserCommand(string Name, string Password);

public class RegisterUserHandler(IAuthService authService)
{
    public async Task<Result<User>> HandleAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<User>.Failure("Name is required");

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result<User>.Failure("Password is required");

        return await authService.RegisterAsync(command.Name, command.Password, ct).ConfigureAwait(false);
    }
}
#pragma warning restore MA0048
