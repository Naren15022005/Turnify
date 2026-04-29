using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Identity.Application.Abstractions;
using Turnify.Modules.Identity.Domain.Entities;
using Turnify.Shared.Kernel.Common;
using RefreshTokenEntity = Turnify.Modules.Identity.Domain.Entities.RefreshToken;

namespace Turnify.Modules.Identity.Application.Users.Commands.RefreshToken;

public sealed class RefreshTokenHandler(IIdentityDbContext db, ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHash = tokenService.HashToken(request.Token);

        var existing = await db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            if (existing is not null)
            {
                existing.Revoke();
                await db.SaveChangesAsync(cancellationToken);
            }
            return Result.Failure<RefreshTokenResponse>(Error.Unauthorized("Refresh token inválido o expirado."));
        }

        var newRawToken = tokenService.GenerateRefreshToken();
        var newHash = tokenService.HashToken(newRawToken);
        var newToken = RefreshTokenEntity.Create(existing.UserId, newHash, request.IpAddress, request.UserAgent);

        db.RefreshTokens.Add(newToken);
        await db.SaveChangesAsync(cancellationToken);

        existing.Revoke(newToken.Id);
        var accessToken = tokenService.GenerateAccessToken(existing.User);
        await db.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken, newRawToken, DateTime.UtcNow.AddMinutes(15));
    }
}
