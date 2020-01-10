FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS http://*:80
RUN apt-get update && apt-get install unzip

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/Services/Account/Account.API
RUN dotnet build "Account.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Account.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Account.API.dll"]
