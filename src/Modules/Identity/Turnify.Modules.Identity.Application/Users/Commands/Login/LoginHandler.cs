using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Identity.Application.Abstractions;
using Turnify.Modules.Identity.Domain.Entities;
using Turnify.Shared.Kernel.Common;
using RefreshTokenEntity = Turnify.Modules.Identity.Domain.Entities.RefreshToken;

namespace Turnify.Modules.Identity.Application.Users.Commands.Login;

public sealed class LoginHandler(
    IIdentityDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private static readonly Error InvalidCredentials =
        Error.Unauthorized("Email o contraseña incorrectos.");

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToUpperInvariant();
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.EmailNormalized == emailNormalized && u.TenantId == request.TenantId, cancellationToken);

        if (user is null)
            return Result.Failure<LoginResponse>(InvalidCredentials);

        if (user.IsLockedOut())
            return Result.Failure<LoginResponse>(
                Error.Unauthorized("Cuenta bloqueada temporalmente. Intente más tarde."));

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await db.SaveChangesAsync(cancellationToken);
            return Result.Failure<LoginResponse>(InvalidCredentials);
        }

        user.RecordSuccessfulLogin();

        var accessToken = tokenService.GenerateAccessToken(user);
        var rawRefreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenHash = tokenService.HashToken(rawRefreshToken);

        var refreshToken = RefreshTokenEntity.Create(user.Id, refreshTokenHash, request.IpAddress, request.UserAgent);
        db.RefreshTokens.Add(refreshToken);
        await db.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            accessToken,
            rawRefreshToken,
            DateTime.UtcNow.AddMinutes(15),
            user.Id,
            user.Email,
            user.TenantId);
    }
}
