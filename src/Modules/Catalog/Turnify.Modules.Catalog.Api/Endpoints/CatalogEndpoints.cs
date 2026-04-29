using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Turnify.Modules.Catalog.Application.Locations.Commands.CreateLocation;
using Turnify.Modules.Catalog.Application.Locations.Queries.GetLocations;
using Turnify.Modules.Catalog.Application.Services.Commands.CreateService;
using Turnify.Modules.Catalog.Application.Services.Queries.GetServices;
using Turnify.Modules.Catalog.Application.Staff.Commands.AssignLocation;
using Turnify.Modules.Catalog.Application.Staff.Commands.AssignService;
using Turnify.Modules.Catalog.Application.Staff.Commands.CreateStaff;
using Turnify.Modules.Catalog.Application.Staff.Queries.GetStaff;
using Turnify.Shared.Kernel.Abstractions;

namespace Turnify.Modules.Catalog.Api.Endpoints;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        MapLocationEndpoints(app);
        MapServiceEndpoints(app);
        MapStaffEndpoints(app);
        return app;
    }

    private static void MapLocationEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/locations").WithTags("Locations").RequireAuthorization();

        group.MapGet("/", async (IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetLocationsQuery(tenant.Id!.Value));
            return Results.Ok(result.Value);
        }).WithName("GetLocations");

        group.MapPost("/", async (CreateLocationCommand command, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(command with { TenantId = tenant.Id!.Value });
            return result.IsSuccess
                ? Results.Created($"/api/locations/{result.Value.PublicId}", result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("CreateLocation");
    }

    private static void MapServiceEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/services").WithTags("Services").RequireAuthorization();

        group.MapGet("/", async (IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetServicesQuery(tenant.Id!.Value));
            return Results.Ok(result.Value);
        }).WithName("GetServices");

        group.MapPost("/", async (CreateServiceCommand command, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(command with { TenantId = tenant.Id!.Value });
            return result.IsSuccess
                ? Results.Created($"/api/services/{result.Value.PublicId}", result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("CreateService");
    }

    private static void MapStaffEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/staff").WithTags("Staff").RequireAuthorization();

        group.MapGet("/", async (IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new GetStaffQuery(tenant.Id!.Value));
            return Results.Ok(result.Value);
        }).WithName("GetStaff");

        group.MapPost("/", async (CreateStaffCommand command, IMediator mediator, ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(command with { TenantId = tenant.Id!.Value });
            return result.IsSuccess
                ? Results.Created($"/api/staff/{result.Value.PublicId}", result.Value)
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("CreateStaff");

        group.MapPost("/{staffPublicId}/services", async (
            string staffPublicId,
            AssignServiceRequest body,
            IMediator mediator,
            ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new AssignServiceToStaffCommand(
                tenant.Id!.Value, staffPublicId, body.ServicePublicId));
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("AssignServiceToStaff");

        group.MapPost("/{staffPublicId}/locations", async (
            string staffPublicId,
            AssignLocationRequest body,
            IMediator mediator,
            ICurrentTenant tenant) =>
        {
            var result = await mediator.Send(new AssignLocationToStaffCommand(
                tenant.Id!.Value, staffPublicId, body.LocationPublicId));
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { result.Error.Code, result.Error.Description });
        }).WithName("AssignLocationToStaff");
    }
}

public sealed record AssignServiceRequest(string ServicePublicId);
public sealed record AssignLocationRequest(string LocationPublicId);
