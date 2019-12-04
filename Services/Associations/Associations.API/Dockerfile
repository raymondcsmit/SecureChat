FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Services/Associations/Associations.API/Associations.API.csproj", "Services/Associations/Associations.API/"]
COPY ["Services/Associations/Associations.Infrastructure/Associations.Infrastructure.csproj", "Services/Associations/Associations.Infrastructure/"]
COPY ["Services/Associations/Associations.Domain/Associations.Domain.csproj", "Services/Associations/Associations.Domain/"]
COPY ["Common/Events/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "Common/Events/EventBusRabbitMQ/"]
COPY ["Common/Helpers/Helpers.csproj", "Common/Helpers/"]
COPY ["Common/Events/EventBus/EventBus.csproj", "Common/Events/EventBus/"]
RUN dotnet restore "Services/Associations/Associations.API/Associations.API.csproj"
COPY . .
WORKDIR "/src/Services/Associations/Associations.API"
RUN dotnet build "Associations.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Associations.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Associations.API.dll"]