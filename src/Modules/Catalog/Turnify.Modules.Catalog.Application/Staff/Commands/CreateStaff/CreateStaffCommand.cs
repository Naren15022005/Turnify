using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Staff.Commands.CreateStaff;

public sealed record CreateStaffCommand(
    long TenantId,
    string FirstName,
    string LastName,
    string? Email = null,
    string? Phone = null,
    string? ProfessionalTitle = null,
    long? UserId = null) : IRequest<Result<StaffDto>>;

public sealed record StaffDto(
    long Id,
    string PublicId,
    string FirstName,
    string LastName,
    string? Email,
    string? Phone,
    string? ProfessionalTitle,
    string? Bio,
    string? PhotoUrl,
    bool IsBookable,
    bool IsActive,
    List<long> ServiceIds,
    List<long> LocationIds);
