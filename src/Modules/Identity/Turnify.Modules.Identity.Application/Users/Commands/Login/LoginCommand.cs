using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Identity.Application.Users.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    long? TenantId = null,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<Result<LoginResponse>>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    long UserId,
    string Email,
    long? TenantId);
