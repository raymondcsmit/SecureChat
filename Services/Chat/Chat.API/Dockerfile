FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
RUN apt-get update && apt-get install unzip

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Services/Chat/Chat.API/Chat.API.csproj", "Services/Chat/Chat.API/"]
COPY ["Services/Chat/Chat.Infrastructure/Chat.Infrastructure.csproj", "Services/Chat/Chat.Infrastructure/"]
COPY ["Services/Chat/Chat.Domain/Chat.Domain.csproj", "Services/Chat/Chat.Domain/"]
COPY ["Common/Events/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "Common/Events/EventBusRabbitMQ/"]
COPY ["Common/Helpers/Helpers.csproj", "Common/Helpers/"]
COPY ["Common/Events/EventBus/EventBus.csproj", "Common/Events/EventBus/"]
RUN dotnet restore "Services/Chat/Chat.API/Chat.API.csproj"
COPY . .
WORKDIR "/src/Services/Chat/Chat.API"
RUN dotnet build "Chat.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Chat.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Chat.API.dll"]