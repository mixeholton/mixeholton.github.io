using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Komit.SimpleApiAuth.TokenProvider;

public class TokenProvider
{
    protected TokenProviderOptions Options;
    public TokenProvider(TokenProviderOptions options) => Options = options;
    public string GenerateToken(Dictionary<string, string> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Options.Secret);
        // ToDo consider adding audiences from options as Aud Claims to get full list of audiences in token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value))),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = Options.Audiences.Single(),
            Issuer = Options.Issuer,
            Expires = DateTime.UtcNow.Add(Options.AccessTokenTtl)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}