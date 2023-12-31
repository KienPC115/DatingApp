// dotnet dev-certs https --trust -> to uss a https
// dotnet tool install --global dotnet-ef --version 7.0.10 -> to use migration
// dotnet ef migrations add InitialCreate -o Data/Migrations -> to create a first migration

using API.Data;
using API.Entities;
using API.Extentions;
using API.Middleware;
using API.SignalR;
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
app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials() // to support a SignalR
    .WithOrigins("https://localhost:4200"));

// ask the user: Do you have a valid token? / Who are you?
app.UseAuthentication(); // it should add bwt UseCors(), MapController()

// ask the user have a valid token => Now what are you allowed to do?
app.UseAuthorization();

app.UseDefaultFiles(); // this is going to fish out the index.html from wwwroot folder
app.UseStaticFiles(); // this is going to look for a wwwroot and serve the content

app.MapControllers();

app.MapHub<PresenceHub>("hubs/presence"); // giống 1 cái endpoint được định nghĩa trong our application
app.MapHub<MessageHub>("hubs/message"); // giống 1 cái endpoint được định nghĩa trong our application
app.MapFallbackToController("Index", "Fallback"); // The request will be routed to a controller endpoint that matches action, and controller.

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

    // we need to clear up the Connection table, because when we restart the API/server -> the user can reconnect with the message hub and
    // when the API restart, the OnDiconnectedAsync() method of message hub does not call -> so that the connection table was not clear and will add more re-connectionId
    // -> cause when the user was out chat box -> but the message of another still be mark as read
    // -> so that when the API restart -> we need to clear up the Connection table
    // context.Connections.RemoveRange(context.Connections); -> this way can clear up the table but if it has 10b row -> cause a problem when start or restart
    // await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]");
    await Seed.ClearConnections(context);
    
    await Seed.SeedUsers(userManager, roleManager);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex,"An error occured during migrations");
}

app.Run();

// DOCKER
// this cli is create a new instance of postgres in the docker container with name, enviroment variable, port, and version
// docker run --name postgres -e POSTGRES_PASSWORD=postgrespw -p 5432:5432 -d postgres:latest

// dockerize our app
// set up Dockerfile and .dockerignore to create new image of our application into docker container
// using cli: docker build -t kientrung1105/datingapp . -> to create the new image
//  docker run --rm -it -p 8080:80 kientrung1105/datingapp:latest -> to run the application by docker
// docker push kientrung1105/datingapp:latest -> to push into docker hub repository

// Fly.io
// after we install fly, login success
// cli: fly launch --image kientrung1105/datingapp:latest -> to lauch our into fly
