# Use .NET 9 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app

# Use .NET 9 runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "SimpleCrudApp.dll"]
