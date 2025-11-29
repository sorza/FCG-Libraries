FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../FCG-Libraries.Api/FCG-Libraries.Api.csproj ./FCG-Libraries.Api/
COPY ../FCG-Libraries.Domain/FCG-Libraries.Domain.csproj ./FCG-Libraries.Domain/
COPY ../FCG-Libraries.Application/FCG-Libraries.Application.csproj ./FCG-Libraries.Application/
COPY ../FCG-Libraries.Infrastructure/FCG-Libraries.Infrastructure.csproj ./FCG-Libraries.Infrastructure/

RUN dotnet restore ./FCG-Libraries.Api/FCG-Libraries.Api.csproj

COPY .. .

RUN dotnet publish ./FCG-Libraries.Api/FCG-Libraries.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG-Libraries.Api.dll"]
