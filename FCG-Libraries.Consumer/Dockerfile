FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FCG-Libraries.Consumer/FCG-Libraries.Consumer.csproj", "FCG-Libraries.Consumer/"]
COPY ["FCG-Libraries.Application/FCG-Libraries.Application.csproj", "FCG-Libraries.Application/"]
COPY ["FCG-Libraries.Domain/FCG-Libraries.Domain.csproj", "FCG-Libraries.Domain/"]
COPY ["FCG-Libraries.Infrastructure/FCG-Libraries.Infrastructure.csproj", "FCG-Libraries.Infrastructure/"]

RUN dotnet restore "FCG-Libraries.Consumer/FCG-Libraries.Consumer.csproj"

COPY . ./
WORKDIR "/src/FCG-Libraries.Consumer"
RUN dotnet publish "FCG-Libraries.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "FCG-Libraries.Consumer.dll"]
