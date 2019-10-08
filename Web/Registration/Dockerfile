FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS http://*:80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/Web/Registration
RUN dotnet build "Registration.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Registration.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Registration.dll"]
