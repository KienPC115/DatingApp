using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    // We use this service to help the application create a token.
    // add package System.IdentityModel.Tokens.Jwt on the Nuget.
    Task<string> CreateToken(AppUser user);
}