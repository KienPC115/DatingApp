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
        services.AddIdentityCore<AppUser>(opt =>
        {
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

                // Add a new option for how are we going to authenticate inside signalR ~ nôm na là authenticate cho endpoint /hubs tương tự như các route-endpoint trong controller
                option.Events = new JwtBearerEvents
                {
                    // context là một đối tượng của lớp MessageReceivedContext -> được tạo ra và truyền vào hàm xử lý của sự kiện, và từ đó, bạn có thể truy cập các thông tin như yêu cầu (Request), ngữ cảnh HTTP (HttpContext), và nhiều thông tin khác.
                    OnMessageReceived = context =>
                    {
                        // Vì đây là yêu cầu cho WebSocket
                        // we cannot pass this up as HTTP Header, we need to pass this up as a query string, and now we get it
                        /*
                        nếu client sử dụng SignalR để gửi và nhận thông điệp real-time thông qua WebSocket(là 1 giao thức liên lạc 2 chiều giữa client với server, truyền tải message real-time) 
                        hoặc các giao thức khác, token JWT có thể được gửi trong query string của yêu cầu WebSocket. 
                        Trong trường hợp này, sự kiện OnMessageReceived sẽ được kích hoạt để xử lý token từ query string và thiết lập nó cho việc xác thực.
                        */
                        var accessToken = context.Request.Query["access_token"]; // access_token - is default string for token in query string

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken; // gắn token cho token của context
                        }
                        // return Task.CompletedTask because this is a delegate Func<MessageReceivedContext, Task>
                        return Task.CompletedTask;
                    }
                };
            });
        // this is techically inside our services, container authentication always come first and then we do authorization would probably be fine
        services.AddAuthorization(opt =>
        {
            // we've got a lot of flexibility inside here about what exactly we want to do
            // we can require that it's just an authenticated user that's required or never. Eg here we require role
            opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
            opt.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator")); // just one role of list can be authorization
        });

        return services;
    }
}