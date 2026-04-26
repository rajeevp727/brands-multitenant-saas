# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY Vendor.API/Vendor.API.csproj Vendor.API/
COPY Vendor.Application/Vendor.Application.csproj Vendor.Application/
COPY Vendor.Domain/Vendor.Domain.csproj Vendor.Domain/
COPY Vendor.Infrastructure/Vendor.Infrastructure.csproj Vendor.Infrastructure/

# Restore dependencies
RUN dotnet restore Vendor.API/Vendor.API.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/Vendor.API
RUN dotnet build "Vendor.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Vendor.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Expose port 80
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Start the application
ENTRYPOINT ["dotnet", "Vendor.API.dll"]
