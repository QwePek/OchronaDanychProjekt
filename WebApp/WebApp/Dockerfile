FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApp/WebApp/WebApp.csproj", "WebApp/WebApp/"]
COPY ["WebApp/WebApp.Client/WebApp.Client.csproj", "WebApp/WebApp.Client/"]
COPY ["WebApp/WebApp.Shared/WebApp.Shared.csproj", "WebApp/WebApp.Shared/"]
COPY ["WebApp/WebApp.DataSeeder/WebApp.DataSeeder.csproj", "WebApp/WebApp.DataSeeder/"]
RUN dotnet restore "./WebApp/WebApp/./WebApp.csproj"
COPY . .
WORKDIR "/src/WebApp/WebApp"
RUN dotnet build "./WebApp.csproj" -c %BUILD_CONFIGURATION% -o /app/build
RUN dotnet tool restore
RUN dotnet ef migrations add Init

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]