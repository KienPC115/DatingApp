using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
// /api/users
[Authorize] // this attribute in here is the controller-level -> will be set all the endpoint with this attribute -> [AllowAnonymus] can be use at same controller
// nhưng ngược lại thì không [AllowAnonymus] at controller-level -> will ignore the [Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
       var users = await _userRepository.GetMembersAsync();
       return Ok(users);
    }

    // /api/users/2
    [HttpGet("{username}")] //Authentication our endpoint by the token 
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username); 
    }


    [HttpPut] // we dont need to add username in parameter root, we can get it from the token
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) {
        // the User of System.Security.Claims.ClaimsPrincipal
        // it contains all the claims of token from client
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if(user==null) return NotFound();

        // this will be map memberUpdateDto to user get from the DB
        // effectively updating all of the properties that we pass through in that memberUpdateDto 
        //into and overwriting the properties in that user.
        _mapper.Map(memberUpdateDto,user);

        if(await _userRepository.SaveAllAsync()) return NoContent();

        // if we map the properties the same with properties had stored in DB
        // EF tracker will not consider it is change and Failed to update.
        return BadRequest("Failed to update user");


    }

}