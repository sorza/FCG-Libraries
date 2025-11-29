FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ../FCG-Libraries.WorkService/FCG-Libraries.WorkService.csproj ./FCG-Libraries.WorkService/
COPY ../FCG-Libraries.Domain/FCG-Libraries.Domain.csproj ./FCG-Libraries.Domain/
COPY ../FCG-Libraries.Application/FCG-Libraries.Application.csproj ./FCG-Libraries.Application/
COPY ../FCG-Libraries.Infrastructure/FCG-Libraries.Infrastructure.csproj ./FCG-Libraries.Infrastructure/

RUN dotnet restore ./FCG-Libraries.WorkService/FCG-Libraries.WorkService.csproj

# Copia todo o código
COPY . .

RUN dotnet publish ./FCG-Libraries.WorkService/FCG-Libraries.WorkService.csproj -c Release -o /app/publish

# Runtime: apenas .NET Core (sem ASP.NET)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG-Libraries.WorkService.dll"]
