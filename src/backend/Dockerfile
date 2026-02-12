# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY SaaS.Api/SaaS.Api.csproj SaaS.Api/
COPY SaaS.Application/SaaS.Application.csproj SaaS.Application/
COPY SaaS.Domain/SaaS.Domain.csproj SaaS.Domain/
COPY SaaS.Infrastructure/SaaS.Infrastructure.csproj SaaS.Infrastructure/

# Restore dependencies
RUN dotnet restore SaaS.Api/SaaS.Api.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/SaaS.Api
RUN dotnet build "SaaS.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "SaaS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Expose dynamic port
ENV ASPNETCORE_URLS=http://+:${PORT:-80}
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "SaaS.Api.dll"]
