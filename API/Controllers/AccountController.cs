using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public AccountController(DataContext context, IUserRepository userRepository, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")] //POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            System.Console.WriteLine(registerDto.KnowAs);

            if (await UserExist(registerDto.UserName))
            {
                return BadRequest("Username is taken");
            }

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512(); // use the .net object to hash a password with hash algorith its

            user.UserName = registerDto.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            _context.Users.Add(user);

            await _context.SaveChangesAsync();
            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnowAs = user.KnowAs
            };
        }

        [HttpPost("login")] //Post: api/account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {

            // var user = await _context.Users.SingleOrDefaultAsync(
            //     user => user.UserName == loginDto.Username);

            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

            if (user is null) return Unauthorized("Invalid username");

            // will get the hashed version of this password
            using var hmac = new HMACSHA512(user.PasswordSalt); // add the passSalt(key) to can generate the same hash with when we register

            // hash the given password -> then compare it to PasswordHash
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }

            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnowAs = user.KnowAs
            };
        }


        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

    }
}