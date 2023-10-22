using API.Data;
using API.Helper;
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
        // add the IOption CloudinarySettings base on which being setup in appsettings.json
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        // add IPhotoService to help us upload/delete photo in Cloud
        services.AddScoped<IPhotoService,PhotoService>();
        // the LogUserActivity it will change the lastActive property of User
        services.AddScoped<LogUserActivity>();    
        // add ILikesRepository into IServiceCollection
        services.AddScoped<ILikesRepository, LikesRepository>();

        return services;
    }


}