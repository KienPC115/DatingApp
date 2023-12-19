using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] //POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            var user = _mapper.Map<AppUser>(registerDto);

            // using var hmac = new HMACSHA512(); // use the .net object to hash a password with hash algorith its

            user.UserName = registerDto.UserName.ToLower();
            // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PasswordSalt = hmac.Key;

            // saves this user into our database -> with auto hash Password by Microsoft
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnowAs = user.KnowAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")] //Post: api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            // var user = await _context.Users.SingleOrDefaultAsync(
            //     user => user.UserName == loginDto.Username);

            var user = await _userManager.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user is null) return Unauthorized("Invalid username");

            // this is ASP.NET Identity to check that password on our behalf
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            // will get the hashed version of this password
            // using var hmac = new HMACSHA512(user.PasswordSalt); // add the passSalt(key) to can generate the same hash with when we register

            // hash the given password -> then compare it to PasswordHash
            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // for (int i = 0; i < computedHash.Length; i++)
            // {
            //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            // }

            if (!result) return Unauthorized("Invalid password");

            return new UserDto()
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnowAs = user.KnowAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

    }
}