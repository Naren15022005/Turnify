using Serilog;
using Turnify.Modules.Booking.Api.Endpoints;
using Turnify.Modules.Booking.Infrastructure;
using Turnify.Modules.Catalog.Api.Endpoints;
using Turnify.Modules.Catalog.Infrastructure;
using Turnify.Modules.Identity.Api.Endpoints;
using Turnify.Modules.Identity.Infrastructure;
using Turnify.Modules.Scheduling.Api.Endpoints;
using Turnify.Modules.Scheduling.Infrastructure;
using Turnify.Modules.Tenants.Api.Endpoints;
using Turnify.Modules.Tenants.Infrastructure;
using Turnify.Shared.Infrastructure.Services;
using Turnify.Shared.Kernel.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, config) =>
    config.ReadFrom.Configuration(ctx.Configuration));

// HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentTenant, CurrentTenant>();

// Módulos
builder.Services.AddTenantsModule(builder.Configuration);
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCatalogModule(builder.Configuration);
builder.Services.AddSchedulingModule(builder.Configuration);
builder.Services.AddBookingModule(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints de módulos
app.MapTenantsEndpoints();
app.MapIdentityEndpoints();
app.MapCatalogEndpoints();
app.MapSchedulingEndpoints();
app.MapBookingEndpoints();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .AllowAnonymous();

app.Run();

public partial class Program { }
