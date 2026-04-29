using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Turnify.Modules.Tenants.Application.Tenants.Queries.GetTenant;

namespace Turnify.Modules.Tenants.Api.Endpoints;

public static class TenantsEndpoints
{
    public static IEndpointRouteBuilder MapTenantsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants").WithTags("Tenants");

        group.MapGet("/{publicId}", async (string publicId, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetTenantQuery(publicId));
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { result.Error.Code, result.Error.Description });
        })
        .WithName("GetTenant")
        .RequireAuthorization();

        return app;
    }
}
