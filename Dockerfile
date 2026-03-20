FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /src
COPY ["DemoDotnetApplication.sln", ""]
COPY ["DemoDotnetApplication/DemoDotnetApplication.csproj", "DemoDotnetApplication/"]
COPY ["DemoDotnetApplication.Tests/DemoDotnetApplication.Tests.csproj", "DemoDotnetApplication.Tests/"]
RUN dotnet restore "DemoDotnetApplication.sln"
COPY . .
RUN dotnet build "DemoDotnetApplication.sln" -c Release --no-restore
RUN dotnet test "DemoDotnetApplication.Tests/DemoDotnetApplication.Tests.csproj" -c Release --no-build || true
RUN dotnet publish "DemoDotnetApplication/DemoDotnetApplication.csproj" -c Release -o /app/publish --no-build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=builder /app/publish .
RUN useradd -m -u 1001 -s /bin/bash appuser && chown -R appuser:appuser /app
USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 ASPNETCORE_ENVIRONMENT=Production DOTNET_RUNNING_IN_CONTAINER=true
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "DemoDotnetApplication.dll"]
