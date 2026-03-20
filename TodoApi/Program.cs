using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("TodoDb"));

builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
