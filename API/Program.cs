// dotnet dev-certs https --trust -> to uss a https
// dotnet tool install --global dotnet-ef --version 7.0.10 -> to use migration
// dotnet ef migrations add InitialCreate -o Data/Migrations -> to create a first migration

using API.Data;
using API.Entities;
using API.Extentions;
using API.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Move some service into the extension method
// in extension method: AddApplicationServices
builder.Services.AddApplicationServices(builder.Configuration);
// in extension method: AddIdentityServices
builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.

// use middleware ExceptionMiddleware catch the exception occur in our application
app.UseMiddleware<ExceptionMiddleware>();

// help this origins route access to the data from this API
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

// ask the user: Do you have a valid token? / Who are you?
app.UseAuthentication(); // it should add bwt UseCors(), MapController()

// ask the user have a valid token => Now what are you allowed to do?
app.UseAuthorization();

app.MapControllers();

// IServiceScope -> IServiceProvider
// give us access to all of the services that we have inside this program class
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    // apply any pending migrations (pending mode) for the context to db, will create db if it does not exist.
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex,"An error occured during migrations");
}

app.Run();
