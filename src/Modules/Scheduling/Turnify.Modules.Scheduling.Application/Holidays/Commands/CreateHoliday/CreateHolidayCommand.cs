using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Scheduling.Application.Holidays.Commands.CreateHoliday;

public sealed record HolidayDto(string PublicId, DateOnly Date, string Name, bool IsRecurring);

public sealed record CreateHolidayCommand(
    long TenantId,
    DateOnly Date,
    string Name,
    bool IsRecurring = false) : IRequest<Result<HolidayDto>>;

public sealed class CreateHolidayHandler(ISchedulingDbContext db)
    : IRequestHandler<CreateHolidayCommand, Result<HolidayDto>>
{
    public async Task<Result<HolidayDto>> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        var exists = await db.Holidays.AnyAsync(
            h => h.TenantId == request.TenantId && h.Date == request.Date, cancellationToken);

        if (exists)
            return Result.Failure<HolidayDto>(Error.Conflict("Holiday.DuplicateDate", $"A holiday already exists for {request.Date}."));

        var holiday = Holiday.Create(request.TenantId, request.Date, request.Name, request.IsRecurring);
        db.Holidays.Add(holiday);
        await db.SaveChangesAsync(cancellationToken);

        return new HolidayDto(holiday.PublicId, holiday.Date, holiday.Name, holiday.IsRecurring);
    }
}
