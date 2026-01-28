ARG BUILD_VERSION=1.0.0
ARG BUILD_DATE
ARG VCS_REF

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["FCG-Libraries.Api/FCG-Libraries.Api.csproj", "FCG-Libraries.Api/"]
COPY ["FCG-Libraries.Application/FCG-Libraries.Application.csproj", "FCG-Libraries.Application/"]
COPY ["FCG-Libraries.Domain/FCG-Libraries.Domain.csproj", "FCG-Libraries.Domain/"]
COPY ["FCG-Libraries.Infrastructure/FCG-Libraries.Infrastructure.csproj", "FCG-Libraries.Infrastructure/"]

RUN dotnet restore "FCG-Libraries.Api/FCG-Libraries.Api.csproj"

COPY . ./
WORKDIR "/src/FCG-Libraries.Api"

RUN dotnet publish "FCG-Libraries.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:InvariantGlobalization=true \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime

LABEL maintainer="FCG Team" \
      version="${BUILD_VERSION}" \
      description="FCG Libraries API - Microservice for user game libraries" \
      org.opencontainers.image.created="${BUILD_DATE}" \
      org.opencontainers.image.revision="${VCS_REF}" \
      org.opencontainers.image.source="https://github.com/alexandresorza/fcg-microservices"

WORKDIR /app

RUN addgroup -g 1000 appuser && \
    adduser -u 1000 -G appuser -s /bin/sh -D appuser && \
    chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish ./

USER appuser

ENV ASPNETCORE_URLS="http://+:80" \
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "FCG-Libraries.Api.dll"]
