using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Lib;
using PhoneShop.Middleware;
using PhoneShop.Object;
using PhoneShop.Service;

namespace PhoneShop.Controllers;

[HttpException]
[ApiController]
[Route("api")]
public class UserController : Controller
{
  private readonly ILogger<UserController> _logger;
  private readonly JWT _jwt;
  private readonly UserService _userService;

  public UserController(
      ILogger<UserController> logger,
      IConfiguration config,
      UserService userService
  )
  {
    _logger = logger;
    _jwt = new JWT(config);
    _userService = userService;
  }

  [HttpPost("signin")]
  [Produces("application/json")]
  public async Task<IResult> signin([FromBody] LoginBody body)
  {
    if (body == null)
    {
      return Results.BadRequest("missing body from request");
    }

    Validator.validateRegister(new RegisterBody { Email = body.Email, Password = body.Password });

    var newUser = await _userService.login(body);

    var claims = new[]
    {
            new Claim("uid", newUser.Uid),
            new Claim(ClaimTypes.Email, body.Email),
            new Claim(ClaimTypes.Role, UserRole.DEFAULT)
        };

    var token = $"Bearer {_jwt.generateToken(claims)}";

    return Results.Json(
        new { data = newUser, token }
    );
  }

  [HttpPost("signup")]
  [Produces("application/json")]
  public async Task<IResult> register([FromBody] RegisterBody body)
  {
    if (body == null)
    {
      return Results.BadRequest("missing body from request");
    }

    Validator.validateRegister(body);

    var newUser = await _userService.register(body);

    var claims = new[]
    {
        new Claim("uid", newUser.Uid),
            new Claim(ClaimTypes.Email, body.Email),
            new Claim(ClaimTypes.Role, body.Role ?? UserRole.DEFAULT)
        };

    var token = $"Bearer {_jwt.generateToken(claims)}";

    return Results.Json(
        new { data = newUser, token }
    );
  }

  [HttpGet("user")]
  [Authorize]
  public async Task<IResult> getUser()
  {
    var identity = HttpContext.User.Identity as ClaimsIdentity;
    if (identity != null)
    {
      var userClaims = identity.Claims;
      var uid = userClaims.FirstOrDefault(claim => claim.Type == "uid")?.Value ?? "null";
      return Results.Json(new
      {
        data = await _userService.getUser(uid)
      });
    }
    return Results.Unauthorized();
  }
}
