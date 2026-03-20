# Demo Dotnet Application

A complete .NET 8 application with DevSecOps pipeline.

## Features

- ASP.NET Core 8
- JWT Authentication
- CORS Support
- Security Headers
- Unit Tests
- Docker Support
- GitHub Actions CI/CD

## Quick Start

### Local Development

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build

# Run tests
dotnet test

# Run application
cd DemoDotnetApplication
dotnet run
```

The application will start on `http://localhost:5000`

### Docker

```bash
# Build image
docker build -t demo-dotnet-app:latest .

# Run container
docker run -p 8080:8080 demo-dotnet-app:latest
```

## API Endpoints

- `GET /` - Home
- `GET /health` - Health check
- `GET /api/items` - Get all items
- `GET /api/items/{id}` - Get item by ID
- `POST /api/items` - Create item (requires auth)
- `GET /api/secure` - Secure data (requires auth)

## GitHub Actions

Pipeline runs on every push with:
- Build & compile
- Unit tests
- Docker build
- Code quality checks

