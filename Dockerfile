# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_RUNNING_IN_CONTAINER=true

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Wex.Purchases.slnx", "src/"]
COPY ["src/Wex.Purchases.Api/Wex.Purchases.Api.csproj", "src/Wex.Purchases.Api/"]
COPY ["src/Wex.Purchases.Application/Wex.Purchases.Application.csproj", "src/Wex.Purchases.Application/"]
COPY ["src/Wex.Purchases.Contracts/Wex.Purchases.Contracts.csproj", "src/Wex.Purchases.Contracts/"]
COPY ["src/Wex.Purchases.Domain/Wex.Purchases.Domain.csproj", "src/Wex.Purchases.Domain/"]
COPY ["src/Wex.Purchases.Infrastructure.MySql/Wex.Purchases.Infrastructure.MySql.csproj", "src/Wex.Purchases.Infrastructure.MySql/"]
COPY ["src/Wex.Purchases.Infrastructure.Treasury/Wex.Purchases.Infrastructure.Treasury.csproj", "src/Wex.Purchases.Infrastructure.Treasury/"]

RUN dotnet restore "./src/Wex.Purchases.slnx" --nologo

COPY . .
WORKDIR "/src/src/Wex.Purchases.Api"

RUN dotnet publish "./Wex.Purchases.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
USER $APP_UID

ENTRYPOINT ["dotnet", "Wex.Purchases.Api.dll"]
