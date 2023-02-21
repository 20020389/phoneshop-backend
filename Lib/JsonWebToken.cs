using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PhoneShop.Lib;

public class JWT
{
    private SigningCredentials _credentials;
    private String _issuer;
    private String _audience;

    public JWT(IConfiguration config)
    {
        var privateKey = config["JWT:Key"] ?? "";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        _credentials = credentials;
        _issuer = config["JWT:Issuer"] ?? "";
        _audience = config["JWT:Audience"] ?? "";
    }

    public String generateToken(Claim[] payload, DateTime? expires = null)
    {
        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            payload,
            expires: expires ?? DateTime.Now.AddDays(1),
            signingCredentials: _credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool validateToken(String token)
    {
        var paramaters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _credentials.Key,
        };

        var handler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        try
        {
            handler.ValidateToken(token, paramaters, out securityToken);
        }
        catch (SecurityTokenException e)
        {
            System.Console.WriteLine(e);
            throw new HttpException(e.Message, HttpStatusCode.BadRequest);
        }

        return securityToken != null;
    }
}
