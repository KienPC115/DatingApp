using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extentions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration config)
    {
        // Add the database for application
        services.AddDbContext<DataContext>(opt =>
        {
            // configure the options for this Database
            opt.UseSqlite(config["ConnectionString:DefaultConnection"]);
        });
        // adding CORS support in the API, because angular can access the server.(policy builder)
        services.AddCors();
        // Add Service to ServiceCollection
        services.AddScoped<ITokenService, TokenService>();
        // add IUserRepository into IServiceCollection
        services.AddScoped<IUserRepository, UserRepository>();
        // add AutoMapper implement class for application
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }


}