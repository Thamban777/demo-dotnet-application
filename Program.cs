using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

// Configure Serilog logging
Log.Logger = new LoggerBuilder()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure Logging
    builder.Host.UseSerilog();

    // Add security headers middleware
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("SecurePolicy", policy =>
        {
            policy
                .WithOrigins(builder.Configuration["AllowedOrigins"] ?? "http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // Add authentication
    var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-key-min-32-characters-long!!";
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "YourApp",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "YourApp-Users",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // Add health checks
    builder.Services.AddHealthChecks();

    // Add HTTP security headers
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    var app = builder.Build();

    // Security middleware
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    app.UseCors("SecurePolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    // Security headers middleware
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
        context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
        await next();
    });

    // Map endpoints
    app.MapHealthChecks("/health");

    // Sample secure API endpoints
    app.MapGet("/", () => Results.Ok(new { message = "Welcome to Secure .NET API", version = "1.0.0" }))
        .WithName("GetRoot")
        .WithOpenApi()
        .WithDescription("Returns API information");

    app.MapGet("/api/secure", () => Results.Ok(new { data = "This is secure data" }))
        .WithName("GetSecureData")
        .WithOpenApi()
        .RequireAuthorization()
        .WithDescription("Requires authentication");

    app.MapGet("/api/users/{id}", (int id) => 
    {
        if (id <= 0)
            return Results.BadRequest(new { error = "Invalid user ID" });
        
        return Results.Ok(new { id = id, name = "Sample User", email = "user@example.com" });
    })
    .WithName("GetUser")
    .WithOpenApi()
    .RequireAuthorization()
    .WithDescription("Get user by ID");

    // Error handling
    app.MapFallback(() => Results.NotFound(new { error = "Endpoint not found" }));

    Log.Information("Starting application...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
