FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS http://*:80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/Services/Chat/Chat.API
RUN dotnet build "Chat.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Chat.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Chat.API.dll"]