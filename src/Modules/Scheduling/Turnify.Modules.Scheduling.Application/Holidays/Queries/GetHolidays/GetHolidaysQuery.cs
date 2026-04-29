using MediatR;
using Microsoft.EntityFrameworkCore;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Application.Holidays.Commands.CreateHoliday;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Scheduling.Application.Holidays.Queries.GetHolidays;

public sealed record GetHolidaysQuery(long TenantId, int? Year = null) : IRequest<Result<List<HolidayDto>>>;

public sealed class GetHolidaysHandler(ISchedulingDbContext db)
    : IRequestHandler<GetHolidaysQuery, Result<List<HolidayDto>>>
{
    public async Task<Result<List<HolidayDto>>> Handle(GetHolidaysQuery request, CancellationToken cancellationToken)
    {
        var query = db.Holidays
            .AsNoTracking()
            .Where(h => h.TenantId == request.TenantId);

        if (request.Year.HasValue)
            query = query.Where(h => h.IsRecurring || h.Date.Year == request.Year.Value);

        var holidays = await query.OrderBy(h => h.Date).ToListAsync(cancellationToken);

        return holidays.Select(h => new HolidayDto(h.PublicId, h.Date, h.Name, h.IsRecurring)).ToList();
    }
}
