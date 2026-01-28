ARG BUILD_VERSION=1.0.0
ARG BUILD_DATE
ARG VCS_REF

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY ["FCG-Libraries.Consumer/FCG-Libraries.Consumer.csproj", "FCG-Libraries.Consumer/"]
COPY ["FCG-Libraries.Application/FCG-Libraries.Application.csproj", "FCG-Libraries.Application/"]
COPY ["FCG-Libraries.Domain/FCG-Libraries.Domain.csproj", "FCG-Libraries.Domain/"]
COPY ["FCG-Libraries.Infrastructure/FCG-Libraries.Infrastructure.csproj", "FCG-Libraries.Infrastructure/"]

RUN dotnet restore "FCG-Libraries.Consumer/FCG-Libraries.Consumer.csproj"

COPY . ./
WORKDIR "/src/FCG-Libraries.Consumer"

RUN dotnet publish "FCG-Libraries.Consumer.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:InvariantGlobalization=true \
    --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS runtime

LABEL maintainer="FCG Team" \
      version="${BUILD_VERSION}" \
      description="FCG Libraries Consumer - Event processor for libraries domain" \
      org.opencontainers.image.created="${BUILD_DATE}" \
      org.opencontainers.image.revision="${VCS_REF}" \
      org.opencontainers.image.source="https://github.com/alexandresorza/fcg-microservices"

WORKDIR /app

RUN addgroup -g 1000 appuser && \
    adduser -u 1000 -G appuser -s /bin/sh -D appuser && \
    chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish ./

USER appuser

ENV DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

ENTRYPOINT ["dotnet", "FCG-Libraries.Consumer.dll"]
