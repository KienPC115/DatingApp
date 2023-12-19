using API.Data;
using API.Helper;
using API.Interfaces;
using API.Services;
using API.SignalR;
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
        // services.AddScoped<IUserRepository, UserRepository>();
        // add AutoMapper implement class for application
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        // add the IOption CloudinarySettings base on which being setup in appsettings.json
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        // add IPhotoService to help us upload/delete photo in Cloud
        services.AddScoped<IPhotoService, PhotoService>();
        // the LogUserActivity it will change the lastActive property of User
        services.AddScoped<LogUserActivity>();
        // add ILikesRepository into IServiceCollection
        // services.AddScoped<ILikesRepository, LikesRepository>();
        // add IMessageRepository into IServiceCollection
        // services.AddScoped<IMessageRepository, MessageRepository>();
        // add SignalR into our application
        services.AddSignalR();
        // we want this dictionary to be available application wide for every user that connects to our service.
        // we do not want this to be destroyed once an HTTP request has been completed
        services.AddSingleton<PresenceTracker>();
        // add IUnitOfWork into IServiceCollection to inject into Controller instead own repository
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }


}