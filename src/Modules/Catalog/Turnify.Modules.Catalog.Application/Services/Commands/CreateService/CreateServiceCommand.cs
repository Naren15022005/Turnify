using MediatR;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Services.Commands.CreateService;

public sealed record CreateServiceCommand(
    long TenantId,
    string Name,
    int DurationMinutes,
    decimal Price,
    long? CategoryId = null,
    string? Description = null,
    int BufferBeforeMinutes = 0,
    int BufferAfterMinutes = 0,
    string? ColorHex = null,
    bool RequiresDeposit = false,
    decimal? DepositAmount = null) : IRequest<Result<ServiceDto>>;

public sealed record ServiceDto(
    long Id,
    string PublicId,
    string Name,
    string? Description,
    int DurationMinutes,
    int BufferBeforeMinutes,
    int BufferAfterMinutes,
    decimal Price,
    string? ColorHex,
    bool RequiresDeposit,
    decimal? DepositAmount,
    long? CategoryId,
    bool IsActive);
