using API.Extentions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helper;

// IAsyncActionFilter: we can be a bit more selective about what we want it to apply to
// là một phần của ASP.NET Core Middleware pipeline
// cho phép bạn thực hiện các hành động trước và sau khi một action method trong controller đc thực thi

// ActionFilter: chỉ thực hiện ở mức controller hoặc action method cụ thể
// Middleware:  hoạt động ở mức toàn bộ ứng dụng ASP.NET Core.
// Sử dụng middleware khi bạn cần thực hiện các tác vụ trước và sau khi yêu cầu HTTP xâm nhập ứng dụng, 
//không quan tâm đến controller hoặc action method cụ thể. 
//Middleware có thể ảnh hưởng đến toàn bộ luồng xử lý yêu cầu và phản hồi.
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // the code excute here will be executed before action method of controller

        var resultContext = await next(); // it represent for action method of controller

        // the code excute here will be executed after action method of controller
        if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();

        var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

        var user = await uow.UserRepository.GetUserByIdAsync(userId);

        user.LastActive = DateTime.UtcNow;

        await uow.Complete();        
    }
}