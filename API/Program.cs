// dotnet dev-certs https --trust -> to uss a https
// dotnet tool install --global dotnet-ef --version 7.0.10 -> to use migration
// dotnet ef migrations add InitialCreate -o Data/Migrations -> to create a first migration

using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration["ConnectionString:DefaultConnection"]);
});
// adding CORS support in the API, because angular can access the server.(policy builder)
builder.Services.AddCors();


var app = builder.Build();

// Configure the HTTP request pipeline.
// help this origins route access to the data from this API
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
