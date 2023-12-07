using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
    // we will store the super secret key in Configuration => will be injected by constructor
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;

    // Because the configuration is the Implicit Object in the webapplication, so it will auto inject it to this constructor
    // => so that why dont need to add it to ServiceCollection
    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        _userManager = userManager;
    }

    public async Task<string> CreateToken(AppUser user)
    {
        //setup claim - đặc trưng của người dùng
        var claims = new List<Claim>
        {
            // dạng type: value cho mỗi claim
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            // roles
        };

        // userManager can help us to get Roles of the user with following method
        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
    
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    
    }
}

