using MediatR;
using Turnify.Modules.Catalog.Application.Abstractions;
using Turnify.Modules.Catalog.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Catalog.Application.Services.Commands.CreateService;

public sealed class CreateServiceHandler(ICatalogDbContext db)
    : IRequestHandler<CreateServiceCommand, Result<ServiceDto>>
{
    public async Task<Result<ServiceDto>> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var service = Service.Create(
            request.TenantId, request.Name, request.DurationMinutes, request.Price,
            request.CategoryId, request.Description,
            request.BufferBeforeMinutes, request.BufferAfterMinutes,
            request.ColorHex, request.RequiresDeposit, request.DepositAmount);

        db.Services.Add(service);
        await db.SaveChangesAsync(cancellationToken);

        return ToDto(service);
    }

    internal static ServiceDto ToDto(Service s) => new(
        s.Id, s.PublicId, s.Name, s.Description,
        s.DurationMinutes, s.BufferBeforeMinutes, s.BufferAfterMinutes,
        s.Price, s.ColorHex, s.RequiresDeposit, s.DepositAmount,
        s.CategoryId, s.IsActive);
}
