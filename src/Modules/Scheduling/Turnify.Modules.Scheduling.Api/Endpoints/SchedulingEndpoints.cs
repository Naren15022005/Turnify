using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Turnify.Modules.Scheduling.Application.Holidays.Commands.CreateHoliday;
using Turnify.Modules.Scheduling.Application.Holidays.Queries.GetHolidays;
using Turnify.Modules.Scheduling.Application.Schedules.Commands.SetStaffSchedule;
using Turnify.Modules.Scheduling.Application.Schedules.Queries.GetStaffSchedule;
using Turnify.Modules.Scheduling.Application.TimeOff.Commands.CreateTimeOff;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Scheduling.Api.Endpoints;

public static class SchedulingEndpoints
{
    public static IEndpointRouteBuilder MapSchedulingEndpoints(this IEndpointRouteBuilder app)
    {
        MapStaffScheduleEndpoints(app);
        MapTimeOffEndpoints(app);
        MapHolidayEndpoints(app);
        return app;
    }

    private static void MapStaffScheduleEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/staff/{staffId:long}/schedule")
            .WithTags("Scheduling")
            .RequireAuthorization();

        group.MapGet("/", async (long staffId, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetStaffScheduleQuery(tenant.Id!.Value, staffId));
            return Results.Ok(result.Value);
        }).WithName("GetStaffSchedule");

        group.MapPut("/", async (long staffId, List<DayScheduleDto> slots, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new SetStaffScheduleCommand(tenant.Id!.Value, staffId, slots));
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("SetStaffSchedule");
    }

    private static void MapTimeOffEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/staff/{staffId:long}/time-off")
            .WithTags("Scheduling")
            .RequireAuthorization();

        group.MapPost("/", async (long staffId, CreateTimeOffBody body, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new CreateTimeOffCommand(
                tenant.Id!.Value, staffId, body.StartsAt, body.EndsAt, body.Reason));
            return result.IsSuccess
                ? Results.Created($"/api/staff/{staffId}/time-off/{result.Value.PublicId}", result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("CreateTimeOff");
    }

    private static void MapHolidayEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/holidays")
            .WithTags("Scheduling")
            .RequireAuthorization();

        group.MapGet("/", async (int? year, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetHolidaysQuery(tenant.Id!.Value, year));
            return Results.Ok(result.Value);
        }).WithName("GetHolidays");

        group.MapPost("/", async (CreateHolidayCommand command, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(command with { TenantId = tenant.Id!.Value });
            return result.IsSuccess
                ? Results.Created($"/api/holidays/{result.Value.PublicId}", result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("CreateHoliday");
    }
}

public sealed record CreateTimeOffBody(DateTime StartsAt, DateTime EndsAt, string? Reason = null);
