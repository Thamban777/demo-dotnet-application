# Multi-stage build for .NET application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Copy project files
COPY ["YourApp/YourApp.csproj", "YourApp/"]
COPY ["YourApp.Tests/YourApp.Tests.csproj", "YourApp.Tests/"]

# Restore dependencies
RUN dotnet restore "YourApp/YourApp.csproj"
RUN dotnet restore "YourApp.Tests/YourApp.Tests.csproj"

# Copy source code
COPY . .

# Build the application
RUN dotnet build "YourApp/YourApp.csproj" -c Release --no-restore

# Run tests
RUN dotnet test "YourApp.Tests/YourApp.Tests.csproj" -c Release --no-build --verbosity normal

# Publish the application
RUN dotnet publish "YourApp/YourApp.csproj" -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published application
COPY --from=builder /app/publish .

# Create non-root user
RUN useradd -m -u 1001 appuser && chown -R appuser:appuser /app
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run application
ENTRYPOINT ["dotnet", "YourApp.dll"]
