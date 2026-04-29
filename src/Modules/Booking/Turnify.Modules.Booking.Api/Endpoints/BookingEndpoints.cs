using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Turnify.Modules.Booking.Application.Appointments.Commands.CreateAppointment;
using Turnify.Modules.Booking.Application.Appointments.Queries.GetAppointments;
using Turnify.Modules.Booking.Application.Appointments.Queries.GetAvailableSlots;
using Turnify.Modules.Booking.Domain.Enums;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Booking.Api.Endpoints;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        MapAppointmentEndpoints(app);
        MapAvailabilityEndpoints(app);
        return app;
    }

    private static void MapAppointmentEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/appointments")
            .WithTags("Booking")
            .RequireAuthorization();

        group.MapGet("/", async (
            long? staffId,
            DateOnly? date,
            AppointmentStatus? status,
            IMediator mediator,
            ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetAppointmentsQuery(tenant.Id!.Value, staffId, date, status));
            return Results.Ok(result.Value);
        }).WithName("GetAppointments");

        group.MapPost("/", async (CreateAppointmentCommand command, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(command with { TenantId = tenant.Id!.Value });
            return result.IsSuccess
                ? Results.Created($"/api/appointments/{result.Value.PublicId}", result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("CreateAppointment");
    }

    private static void MapAvailabilityEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/availability")
            .WithTags("Booking");

        group.MapGet("/", async (
            long staffId,
            long serviceId,
            DateOnly date,
            int? slotInterval,
            IMediator mediator,
            ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetAvailableSlotsQuery(
                tenant.Id!.Value, staffId, serviceId, date, slotInterval ?? 15));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("GetAvailableSlots").AllowAnonymous();
    }
}
