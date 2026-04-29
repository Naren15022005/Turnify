using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Locations.Commands.CreateLocation;

public sealed record CreateLocationCommand(
    long TenantId,
    string Name,
    string? Address = null,
    string? City = null,
    string? Department = null,
    string? Phone = null) : IRequest<Result<LocationDto>>;

public sealed record LocationDto(
    long Id,
    string PublicId,
    string Name,
    string? Address,
    string? City,
    string? Department,
    string? Phone,
    bool IsActive);
