#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Services/Chats/Chats.Api/Chats.Api.csproj", "Services/Chats/Chats.Api/"]
COPY ["Common/Events/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "Common/Events/EventBusRabbitMQ/"]
COPY ["Common/Helpers/Helpers.csproj", "Common/Helpers/"]
COPY ["Common/Events/EventBus/EventBus.csproj", "Common/Events/EventBus/"]
RUN dotnet restore "Services/Chats/Chats.Api/Chats.Api.csproj"
COPY . .
WORKDIR "/src/Services/Chats/Chats.Api"
RUN dotnet build "Chats.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Chats.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chats.Api.dll"]