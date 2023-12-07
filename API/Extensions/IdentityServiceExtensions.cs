using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extentions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        // Add IdentityCore for AppUser into application
        services.AddIdentityCore<AppUser>(opt => {
            // here we can setup Password, User, Lockout - khóa user, SignIn - cấu hình đăng nhập 
            opt.Password.RequireNonAlphanumeric = false; // không có kí tự đặc biệt
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>() // add RoleManager to have a services/apis manage roles of user
        .AddEntityFrameworkStores<DataContext>(); //Thêm triển khai EF lưu trữ thông tin về Idetity (theo DataContext -> database).
        
        // add package Microsoft.AspNetCore.Authentication.JwtBearer
        // give our server enough information to take a look at the token based on the issuer signing key
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    // our server will check the token signing key => ensure it is valid
                    // kí vào token
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                    // tự cấp token
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
        // this is techically inside our services, container authentication always come first and then we do authorization would probably be fine
        services.AddAuthorization(opt => {
            // we've got a lot of flexibility inside here about what exactly we want to do
            // we can require that it's just an authenticated user that's required or never. Eg here we require role
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator")); // just one role of list can be authorization
        });

        return services;
    }
}