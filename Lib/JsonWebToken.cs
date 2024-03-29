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

  private int _expires;

  public int expires { get => _expires; }

  public JWT(IConfiguration config)//constructor 
  {
    var privateKey = config["JWT:Key"] ?? "";
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    _credentials = credentials;
    _issuer = config["JWT:Issuer"] ?? "";
    _audience = config["JWT:Audience"] ?? "";
    _expires = 1;
  }

  public String generateToken(Claim[] payload, int? exp = null)
  {
    var token = new JwtSecurityToken(
        _issuer,
        _audience,
        payload,
        expires: DateTime.Now.AddDays(exp ?? _expires),
        signingCredentials: _credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public ClaimsPrincipal validateToken(String token, bool? throwError)
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
      return handler.ValidateToken(token, paramaters, out securityToken);
    }
    catch (Exception)
    {
      throw new HttpException("Unauthorized", HttpStatusCode.Unauthorized);
    }
  }

  public static String useToken(HttpContext context, string key = "uid")
  {
    var identity = context.User.Identity as ClaimsIdentity;
    var userClaims = identity?.Claims;
    var value = userClaims?.FirstOrDefault(claim => claim.Type == key)?.Value;

    if (value == null)
    {
      throw new HttpException("Unauthorized", HttpStatusCode.Unauthorized);
    }

    return value;
  }

  public static String? tryGetToken(HttpContext context, string key = "uid")
  {
    var identity = context.User.Identity as ClaimsIdentity;
    var userClaims = identity?.Claims;
    var value = userClaims?.FirstOrDefault(claim => claim.Type == key)?.Value;
    return value;
  }
}
