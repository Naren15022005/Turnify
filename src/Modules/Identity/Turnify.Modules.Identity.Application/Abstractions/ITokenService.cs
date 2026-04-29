using Turnify.Modules.Identity.Domain.Entities;

namespace Turnify.Modules.Identity.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
}
