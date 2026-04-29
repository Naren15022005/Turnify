using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Turnify.Modules.Identity.Application.Users.Commands.Login;
using Turnify.Modules.Identity.Application.Users.Commands.RefreshToken;
using Turnify.Modules.Identity.Application.Users.Commands.Register;

namespace Turnify.Modules.Identity.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Identity");

        group.MapPost("/register", async (RegisterUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
                ? Results.Created($"/api/users/{result.Value.PublicId}", result.Value)
                : Results.Conflict(new { result.Error.Code, result.Error.Description });
        })
        .WithName("Register")
        .AllowAnonymous();

        group.MapPost("/login", async (LoginCommand command, IMediator mediator, HttpContext ctx) =>
        {
            var commandWithMeta = command with
            {
                IpAddress = ctx.Connection.RemoteIpAddress?.ToString(),
                UserAgent = ctx.Request.Headers.UserAgent
            };
            var result = await mediator.Send(commandWithMeta);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        })
        .WithName("Login")
        .AllowAnonymous();

        group.MapPost("/refresh", async (RefreshTokenCommand command, IMediator mediator, HttpContext ctx) =>
        {
            var commandWithMeta = command with
            {
                IpAddress = ctx.Connection.RemoteIpAddress?.ToString(),
                UserAgent = ctx.Request.Headers.UserAgent
            };
            var result = await mediator.Send(commandWithMeta);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        })
        .WithName("RefreshToken")
        .AllowAnonymous();

        return app;
    }
}
