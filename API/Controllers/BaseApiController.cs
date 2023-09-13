using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Client does not maintain a session(what client has/whatever the client is doing with the API) with our API server -> It's stateless

    // Json Web Tokens (JWT)
    // (Benefits: No session manage -> Jwts are self contained tokens
    // Portable - A single token can be used with multiple backends
    // No Cookies required - mobile friendly
    // Performance - Once a token is issued, no need to make a database request to verify a users authen)
    //-> contain Credentials
    //-> contain Claims -> đăng kí thông tin mà user claims
    //-> other information
    // JWT Structure
    // 3 parts: 
    //HEADER: Algorithm & token type
    //PAYLOAD: Data (such as id, username, roles the user, expired, iat-issued at ...)
    //VERIFY SIGNATURE - chữ kí xác minh

    // use BaseApiController to help the project -> DRY 
    //-> reduce the repeatation, use inheritance that will declare and add all attribute are necessary
    
    // ApiController can automatically bind to our parameters inside code
    // ApiController -> can help validation data before it get this data to controller.(Use Annotation on the Dto)
    [ApiController] 
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
    }


}