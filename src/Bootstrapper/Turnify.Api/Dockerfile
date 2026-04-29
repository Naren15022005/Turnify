FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Bootstrapper/Turnify.Api/Turnify.Api.csproj", "src/Bootstrapper/Turnify.Api/"]
COPY ["src/Shared/Kernel/Turnify.Shared.Kernel/Turnify.Shared.Kernel.csproj", "src/Shared/Kernel/Turnify.Shared.Kernel/"]
COPY ["src/Shared/Infrastructure/Turnify.Shared.Infrastructure/Turnify.Shared.Infrastructure.csproj", "src/Shared/Infrastructure/Turnify.Shared.Infrastructure/"]
COPY ["src/Shared/Contracts/Turnify.Shared.Contracts/Turnify.Shared.Contracts.csproj", "src/Shared/Contracts/Turnify.Shared.Contracts/"]
COPY ["src/Modules/Tenants/Turnify.Modules.Tenants.Domain/Turnify.Modules.Tenants.Domain.csproj", "src/Modules/Tenants/Turnify.Modules.Tenants.Domain/"]
COPY ["src/Modules/Tenants/Turnify.Modules.Tenants.Application/Turnify.Modules.Tenants.Application.csproj", "src/Modules/Tenants/Turnify.Modules.Tenants.Application/"]
COPY ["src/Modules/Tenants/Turnify.Modules.Tenants.Infrastructure/Turnify.Modules.Tenants.Infrastructure.csproj", "src/Modules/Tenants/Turnify.Modules.Tenants.Infrastructure/"]
COPY ["src/Modules/Tenants/Turnify.Modules.Tenants.Api/Turnify.Modules.Tenants.Api.csproj", "src/Modules/Tenants/Turnify.Modules.Tenants.Api/"]
COPY ["src/Modules/Identity/Turnify.Modules.Identity.Domain/Turnify.Modules.Identity.Domain.csproj", "src/Modules/Identity/Turnify.Modules.Identity.Domain/"]
COPY ["src/Modules/Identity/Turnify.Modules.Identity.Application/Turnify.Modules.Identity.Application.csproj", "src/Modules/Identity/Turnify.Modules.Identity.Application/"]
COPY ["src/Modules/Identity/Turnify.Modules.Identity.Infrastructure/Turnify.Modules.Identity.Infrastructure.csproj", "src/Modules/Identity/Turnify.Modules.Identity.Infrastructure/"]
COPY ["src/Modules/Identity/Turnify.Modules.Identity.Api/Turnify.Modules.Identity.Api.csproj", "src/Modules/Identity/Turnify.Modules.Identity.Api/"]
COPY ["src/Modules/Catalog/Turnify.Modules.Catalog.Domain/Turnify.Modules.Catalog.Domain.csproj", "src/Modules/Catalog/Turnify.Modules.Catalog.Domain/"]
COPY ["src/Modules/Catalog/Turnify.Modules.Catalog.Application/Turnify.Modules.Catalog.Application.csproj", "src/Modules/Catalog/Turnify.Modules.Catalog.Application/"]
COPY ["src/Modules/Catalog/Turnify.Modules.Catalog.Infrastructure/Turnify.Modules.Catalog.Infrastructure.csproj", "src/Modules/Catalog/Turnify.Modules.Catalog.Infrastructure/"]
COPY ["src/Modules/Catalog/Turnify.Modules.Catalog.Api/Turnify.Modules.Catalog.Api.csproj", "src/Modules/Catalog/Turnify.Modules.Catalog.Api/"]
COPY ["src/Modules/Scheduling/Turnify.Modules.Scheduling.Domain/Turnify.Modules.Scheduling.Domain.csproj", "src/Modules/Scheduling/Turnify.Modules.Scheduling.Domain/"]
COPY ["src/Modules/Scheduling/Turnify.Modules.Scheduling.Application/Turnify.Modules.Scheduling.Application.csproj", "src/Modules/Scheduling/Turnify.Modules.Scheduling.Application/"]
COPY ["src/Modules/Scheduling/Turnify.Modules.Scheduling.Infrastructure/Turnify.Modules.Scheduling.Infrastructure.csproj", "src/Modules/Scheduling/Turnify.Modules.Scheduling.Infrastructure/"]
COPY ["src/Modules/Scheduling/Turnify.Modules.Scheduling.Api/Turnify.Modules.Scheduling.Api.csproj", "src/Modules/Scheduling/Turnify.Modules.Scheduling.Api/"]
COPY ["src/Modules/Booking/Turnify.Modules.Booking.Domain/Turnify.Modules.Booking.Domain.csproj", "src/Modules/Booking/Turnify.Modules.Booking.Domain/"]
COPY ["src/Modules/Booking/Turnify.Modules.Booking.Application/Turnify.Modules.Booking.Application.csproj", "src/Modules/Booking/Turnify.Modules.Booking.Application/"]
COPY ["src/Modules/Booking/Turnify.Modules.Booking.Infrastructure/Turnify.Modules.Booking.Infrastructure.csproj", "src/Modules/Booking/Turnify.Modules.Booking.Infrastructure/"]
COPY ["src/Modules/Booking/Turnify.Modules.Booking.Api/Turnify.Modules.Booking.Api.csproj", "src/Modules/Booking/Turnify.Modules.Booking.Api/"]
COPY ["src/Modules/Payments/Turnify.Modules.Payments.Domain/Turnify.Modules.Payments.Domain.csproj", "src/Modules/Payments/Turnify.Modules.Payments.Domain/"]
COPY ["src/Modules/Payments/Turnify.Modules.Payments.Application/Turnify.Modules.Payments.Application.csproj", "src/Modules/Payments/Turnify.Modules.Payments.Application/"]
COPY ["src/Modules/Payments/Turnify.Modules.Payments.Infrastructure/Turnify.Modules.Payments.Infrastructure.csproj", "src/Modules/Payments/Turnify.Modules.Payments.Infrastructure/"]
COPY ["src/Modules/Payments/Turnify.Modules.Payments.Api/Turnify.Modules.Payments.Api.csproj", "src/Modules/Payments/Turnify.Modules.Payments.Api/"]
COPY ["src/Modules/Notifications/Turnify.Modules.Notifications.Domain/Turnify.Modules.Notifications.Domain.csproj", "src/Modules/Notifications/Turnify.Modules.Notifications.Domain/"]
COPY ["src/Modules/Notifications/Turnify.Modules.Notifications.Application/Turnify.Modules.Notifications.Application.csproj", "src/Modules/Notifications/Turnify.Modules.Notifications.Application/"]
COPY ["src/Modules/Notifications/Turnify.Modules.Notifications.Infrastructure/Turnify.Modules.Notifications.Infrastructure.csproj", "src/Modules/Notifications/Turnify.Modules.Notifications.Infrastructure/"]
COPY ["src/Modules/Notifications/Turnify.Modules.Notifications.Api/Turnify.Modules.Notifications.Api.csproj", "src/Modules/Notifications/Turnify.Modules.Notifications.Api/"]
COPY ["src/Modules/Reporting/Turnify.Modules.Reporting.Domain/Turnify.Modules.Reporting.Domain.csproj", "src/Modules/Reporting/Turnify.Modules.Reporting.Domain/"]
COPY ["src/Modules/Reporting/Turnify.Modules.Reporting.Application/Turnify.Modules.Reporting.Application.csproj", "src/Modules/Reporting/Turnify.Modules.Reporting.Application/"]
COPY ["src/Modules/Reporting/Turnify.Modules.Reporting.Infrastructure/Turnify.Modules.Reporting.Infrastructure.csproj", "src/Modules/Reporting/Turnify.Modules.Reporting.Infrastructure/"]
COPY ["src/Modules/Reporting/Turnify.Modules.Reporting.Api/Turnify.Modules.Reporting.Api.csproj", "src/Modules/Reporting/Turnify.Modules.Reporting.Api/"]
COPY ["Directory.Build.props", "."]

RUN dotnet restore "src/Bootstrapper/Turnify.Api/Turnify.Api.csproj"

COPY . .
WORKDIR "/src/src/Bootstrapper/Turnify.Api"
RUN dotnet build "Turnify.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Turnify.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Turnify.Api.dll"]
