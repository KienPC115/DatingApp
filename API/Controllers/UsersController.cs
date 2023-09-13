using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
// /api/users
[Authorize] // this attribute in here is the controller-level -> will be set all the endpoint with this attribute -> [AllowAnonymus] can be use at same controller
// nhưng ngược lại thì không [AllowAnonymus] at controller-level -> will ignore the [Authorize]
public class UsersController : BaseApiController
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        this._context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    // /api/users/2
    [HttpGet("{id}")] //Authentication our endpoint by the token 
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        return await _context.Users.FindAsync(id);
        
    }


}