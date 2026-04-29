using MediatR;
using Turnify.Modules.Scheduling.Application.Abstractions;
using Turnify.Modules.Scheduling.Domain.Entities;
using Turnify.Shared.Kernel.Common;

namespace Turnify.Modules.Scheduling.Application.TimeOff.Commands.CreateTimeOff;

public sealed record TimeOffDto(string PublicId, long StaffId, DateTime StartsAt, DateTime EndsAt, string? Reason);

public sealed record CreateTimeOffCommand(
    long TenantId,
    long StaffId,
    DateTime StartsAt,
    DateTime EndsAt,
    string? Reason = null) : IRequest<Result<TimeOffDto>>;

public sealed class CreateTimeOffHandler(ISchedulingDbContext db)
    : IRequestHandler<CreateTimeOffCommand, Result<TimeOffDto>>
{
    public async Task<Result<TimeOffDto>> Handle(CreateTimeOffCommand request, CancellationToken cancellationToken)
    {
        if (request.EndsAt <= request.StartsAt)
            return Result.Failure<TimeOffDto>(Error.Validation("TimeOff.InvalidRange", "EndsAt must be after StartsAt."));

        var timeOff = StaffTimeOff.Create(request.TenantId, request.StaffId,
            request.StartsAt, request.EndsAt, request.Reason);

        db.StaffTimeOffs.Add(timeOff);
        await db.SaveChangesAsync(cancellationToken);

        return new TimeOffDto(timeOff.PublicId, timeOff.StaffId, timeOff.StartsAt, timeOff.EndsAt, timeOff.Reason);
    }
}
