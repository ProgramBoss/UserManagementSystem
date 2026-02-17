using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Api.Data;
using UserManagementSystem.Api.Repositories;
using UserManagementSystem.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "User Management System API",
        Version = "v1",
        Description = "A RESTful API for managing users, groups, and permissions"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI",
        builder => builder
            .WithOrigins("https://localhost:7001", "http://localhost:5001")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Ensuring SQLite database is created...");
        context.Database.EnsureCreated();
        
        logger.LogInformation("SQLite database is ready at: {DbPath}", 
            Path.Combine(Directory.GetCurrentDirectory(), "usermanagement.db"));
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while ensuring the database is created.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowWebUI");

app.UseAuthorization();

app.MapControllers();

app.Run();
