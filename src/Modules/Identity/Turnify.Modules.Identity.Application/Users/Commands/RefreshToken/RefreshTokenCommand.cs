using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Identity.Application.Users.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string Token,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<Result<RefreshTokenResponse>>;

public sealed record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);
