using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Identity.Application.Abstractions;
using Turnify.Modules.Identity.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Identity.Application.Users.Commands.Register;

public sealed class RegisterUserHandler(IIdentityDbContext db, IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    public async Task<Result<RegisterUserResponse>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var emailNormalized = request.Email.Trim().ToUpperInvariant();
        var emailExists = await db.Users
            .AnyAsync(u => u.EmailNormalized == emailNormalized && u.TenantId == request.TenantId, cancellationToken);

        if (emailExists)
            return Result.Failure<RegisterUserResponse>(
                Error.Conflict("User.EmailTaken", $"El email '{request.Email}' ya está registrado."));

        var hash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, hash, request.FirstName, request.LastName, request.TenantId, request.Phone);

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return new RegisterUserResponse(user.Id, user.PublicId, user.Email);
    }
}
