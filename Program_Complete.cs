using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Configure Serilog logging before building the app
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("🚀 Starting Demo Dotnet Application...");
    
    var builder = WebApplication.CreateBuilder(args);

    // Add Serilog to logging
    builder.Host.UseSerilog();

    // ============================================================================
    // SECURITY: CORS Configuration
    // ============================================================================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("SecurePolicy", policy =>
        {
            var allowedOrigins = builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000";
            policy
                .WithOrigins(allowedOrigins.Split(","))
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // ============================================================================
    // SECURITY: JWT Authentication
    // ============================================================================
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-key-minimum-32-characters-long-for-production!!";
    var jwtKey = Encoding.ASCII.GetBytes(jwtSecret);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "DemoDotnetApplication",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "DemoDotnetApplication-Users",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // ============================================================================
    // Health Checks
    // ============================================================================
    builder.Services.AddHealthChecks();

    // ============================================================================
    // SECURITY: HSTS Configuration
    // ============================================================================
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    // Build the application
    var app = builder.Build();

    // ============================================================================
    // SECURITY: HTTP Pipeline
    // ============================================================================
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    app.UseCors("SecurePolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    // ============================================================================
    // SECURITY: Custom Security Headers Middleware
    // ============================================================================
    app.Use(async (context, next) =>
    {
        // Prevent MIME type sniffing
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        
        // Prevent clickjacking
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        
        // Enable XSS protection
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        
        // HSTS header
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        
        // Content Security Policy
        context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline';");
        
        // Remove server header to avoid information disclosure
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        
        await next();
    });

    // ============================================================================
    // API ENDPOINTS
    // ============================================================================

    // Health check endpoint (no authentication required)
    app.MapHealthChecks("/health")
        .WithName("HealthCheck")
        .WithOpenApi()
        .WithDescription("Health check endpoint - used by load balancers");

    // Home endpoint
    app.MapGet("/", GetHomeEndpoint)
        .WithName("GetHome")
        .WithOpenApi()
        .WithDescription("Returns API information and version");

    // Get all items (public)
    app.MapGet("/api/items", GetItemsEndpoint)
        .WithName("GetItems")
        .WithOpenApi()
        .WithDescription("Get all items from the system");

    // Get item by ID (public)
    app.MapGet("/api/items/{id}", GetItemByIdEndpoint)
        .WithName("GetItemById")
        .WithOpenApi()
        .WithDescription("Get a specific item by ID");

    // Create new item (requires authentication)
    app.MapPost("/api/items", CreateItemEndpoint)
        .WithName("CreateItem")
        .WithOpenApi()
        .RequireAuthorization()
        .WithDescription("Create a new item - requires authentication");

    // Update item (requires authentication)
    app.MapPut("/api/items/{id}", UpdateItemEndpoint)
        .WithName("UpdateItem")
        .WithOpenApi()
        .RequireAuthorization()
        .WithDescription("Update an item - requires authentication");

    // Delete item (requires authentication)
    app.MapDelete("/api/items/{id}", DeleteItemEndpoint)
        .WithName("DeleteItem")
        .WithOpenApi()
        .RequireAuthorization()
        .WithDescription("Delete an item - requires authentication");

    // Secure data endpoint
    app.MapGet("/api/secure", GetSecureDataEndpoint)
        .WithName("GetSecureData")
        .WithOpenApi()
        .RequireAuthorization()
        .WithDescription("Get secure data - requires authentication");

    // Error handling for undefined routes
    app.MapFallback(() => Results.NotFound(new { error = "Endpoint not found", status = 404 }));

    Log.Information("✅ Demo Dotnet Application configured successfully");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}

// ============================================================================
// ENDPOINT HANDLERS
// ============================================================================

// Home endpoint handler
IResult GetHomeEndpoint()
{
    return Results.Ok(new
    {
        message = "Welcome to Demo Dotnet Application",
        version = "1.0.0",
        environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
        timestamp = DateTime.UtcNow,
        documentation = "/swagger/index.html"
    });
}

// Get all items
IResult GetItemsEndpoint()
{
    var items = new[]
    {
        new { id = 1, name = "Item 1", description = "First demo item", createdAt = DateTime.UtcNow.AddDays(-5) },
        new { id = 2, name = "Item 2", description = "Second demo item", createdAt = DateTime.UtcNow.AddDays(-3) },
        new { id = 3, name = "Item 3", description = "Third demo item", createdAt = DateTime.UtcNow.AddDays(-1) }
    };
    
    return Results.Ok(new { data = items, count = items.Length });
}

// Get item by ID
IResult GetItemByIdEndpoint(int id)
{
    // Input validation
    if (id <= 0)
        return Results.BadRequest(new { error = "Invalid item ID", status = 400 });

    // Find item (demo data)
    var items = new[]
    {
        new { id = 1, name = "Item 1", description = "First demo item", createdAt = DateTime.UtcNow.AddDays(-5) },
        new { id = 2, name = "Item 2", description = "Second demo item", createdAt = DateTime.UtcNow.AddDays(-3) },
        new { id = 3, name = "Item 3", description = "Third demo item", createdAt = DateTime.UtcNow.AddDays(-1) }
    };

    var item = items.FirstOrDefault(x => x.id == id);
    
    if (item == null)
        return Results.NotFound(new { error = "Item not found", status = 404 });

    return Results.Ok(item);
}

// Create new item
IResult CreateItemEndpoint(CreateItemRequest request, ILogger<Program> logger)
{
    logger.LogInformation("📝 Creating new item: {ItemName}", request.Name);

    // Validation
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Item name is required", status = 400 });

    if (request.Name.Length > 100)
        return Results.BadRequest(new { error = "Item name must be less than 100 characters", status = 400 });

    var newItem = new
    {
        id = 4,
        name = request.Name,
        description = request.Description ?? string.Empty,
        createdAt = DateTime.UtcNow
    };

    logger.LogInformation("✅ Item created successfully");
    return Results.Created($"/api/items/{newItem.id}", newItem);
}

// Update item
IResult UpdateItemEndpoint(int id, UpdateItemRequest request, ILogger<Program> logger)
{
    if (id <= 0)
        return Results.BadRequest(new { error = "Invalid item ID", status = 400 });

    logger.LogInformation("📝 Updating item {ItemId}", id);

    var updatedItem = new
    {
        id = id,
        name = request.Name ?? $"Item {id}",
        description = request.Description ?? string.Empty,
        updatedAt = DateTime.UtcNow
    };

    return Results.Ok(updatedItem);
}

// Delete item
IResult DeleteItemEndpoint(int id, ILogger<Program> logger)
{
    if (id <= 0)
        return Results.BadRequest(new { error = "Invalid item ID", status = 400 });

    logger.LogInformation("🗑️  Deleting item {ItemId}", id);
    return Results.NoContent();
}

// Get secure data (requires authentication)
IResult GetSecureDataEndpoint(HttpContext context, ILogger<Program> logger)
{
    var user = context.User.Identity?.Name ?? "Unknown";
    logger.LogInformation("🔐 Secure data accessed by user: {User}", user);

    return Results.Ok(new
    {
        message = "This is secure data",
        user = user,
        timestamp = DateTime.UtcNow,
        data = new { secret = "Demo secure information" }
    });
}

// ============================================================================
// REQUEST/RESPONSE MODELS
// ============================================================================

public class CreateItemRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateItemRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}
