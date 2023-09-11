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



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
