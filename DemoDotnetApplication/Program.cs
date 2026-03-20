using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("🚀 Starting Demo Dotnet Application...");
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("SecurePolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    // JWT Authentication
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-key-minimum-32-characters-long!!";
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
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors("SecurePolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    // Security Headers
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        await next();
    });

    // Health Check
    app.MapHealthChecks("/health");

    // Root endpoint
    app.MapGet("/", () => Results.Ok(new 
    { 
        message = "Welcome to Demo Dotnet Application", 
        version = "1.0.0",
        environment = app.Environment.EnvironmentName
    }))
    .WithName("GetHome")
    .WithOpenApi();

    // Get all items
    app.MapGet("/api/items", () => Results.Ok(new 
    { 
        items = new[] 
        { 
            new { id = 1, name = "Item 1" },
            new { id = 2, name = "Item 2" }
        ] 
    }))
    .WithName("GetItems")
    .WithOpenApi();

    // Get item by ID
    app.MapGet("/api/items/{id}", (int id) => 
        id <= 0 ? Results.BadRequest(new { error = "Invalid ID" }) 
                : Results.Ok(new { id = id, name = $"Item {id}" }))
    .WithName("GetItemById")
    .WithOpenApi();

    // Create item (protected)
    app.MapPost("/api/items", (ItemRequest request) => 
        Results.Created($"/api/items/1", new { id = 1, name = request.Name }))
    .WithName("CreateItem")
    .WithOpenApi()
    .RequireAuthorization();

    // Secure endpoint
    app.MapGet("/api/secure", () => 
        Results.Ok(new { message = "This is secure data", timestamp = DateTime.UtcNow }))
    .WithName("SecureData")
    .WithOpenApi()
    .RequireAuthorization();

    Log.Information("✅ Application configured successfully");
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

public class ItemRequest
{
    public string Name { get; set; } = string.Empty;
}
