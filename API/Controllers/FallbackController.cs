using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// What does our API need to fallback to if it doesn't know how
//to hanlde a specific route.
// Khi ta deploy lên refesh lại trang mà route đó không đc API server hỗ trợ -> lỗi NotFound
//=> vì thế ta add FallBack controller -> để lấy angular cho nó handle the rooting
public class FallbackController : Controller // use Controller -> because we need access to a view and effectively we're going to tell it to go to our physical file. eg: index.html
{
    public ActionResult Index() {
        return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), // GetCurrentDirectory -> get our API folder
            "wwwroot","index.html"), "text/HTML");
    }
}