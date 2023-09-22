// dotnet dev-certs https --trust -> to uss a https
// dotnet tool install --global dotnet-ef --version 7.0.10 -> to use migration
// dotnet ef migrations add InitialCreate -o Data/Migrations -> to create a first migration

using API.Extentions;
using API.Middleware;

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

app.Run();
