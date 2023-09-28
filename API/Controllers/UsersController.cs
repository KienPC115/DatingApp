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


}