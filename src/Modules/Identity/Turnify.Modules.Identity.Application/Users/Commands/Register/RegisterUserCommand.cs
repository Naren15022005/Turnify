using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Identity.Application.Users.Commands.Register;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    long? TenantId = null,
    string? Phone = null) : IRequest<Result<RegisterUserResponse>>;

public sealed record RegisterUserResponse(long UserId, string PublicId, string Email);
