using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Lib;
using PhoneShop.Middleware;
using PhoneShop.Interface;
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

    var userResponse = newUser.parse<UserWithoutPassword>();

    if (userResponse != null)
    {
      userResponse.Password = null;
    }

    return Results.Json(
        new { data = userResponse, token }
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

    var userResponse = newUser.parse<UserWithoutPassword>();

    if (userResponse != null)
    {
      userResponse.Password = null;
    }

    return Results.Json(
        new { data = userResponse, token }
    );
  }

  [HttpGet("user")]
  [Authorize]
  public async Task<IResult> getUser()
  {
    var uid = JWT.useToken(HttpContext);
    var newUser = await _userService.getUser(uid);
    var userResponse = newUser.parse<UserWithoutPassword>();

    if (userResponse != null)
    {
      userResponse.Password = null;
    }

    return Results.Json(
        new { data = userResponse }
    );
  }

  [HttpPost("user")]
  [Authorize]
  public async Task<IResult> updateUser([FromBody] UpdateUserBody body)
  {
    var uid = JWT.useToken(HttpContext);
    var newUser = await _userService.updateUser(uid, body);
    var userResponse = newUser.parse<UserWithoutPassword>();

    if (userResponse != null)
    {
      userResponse.Password = null;
    }

    return Results.Json(
        new { data = userResponse }
    );
  }

  [HttpPost("user/cart")]
  [Authorize(Roles = UserRole.DEFAULT)]
  public async Task<IResult> addProductToCart([FromBody] RemoveProductFromCartBody body)
  {
    var uid = JWT.useToken(HttpContext);
    await _userService.addProductToCart(uid, body);

    return Results.Json(
        new
        {
          message = "success"
        }
    );
  }

  [HttpGet("user/cart")]
  [Authorize(Roles = UserRole.DEFAULT)]
  public async Task<IResult> getCart()
  {
    var uid = JWT.useToken(HttpContext);
    var data = await _userService.getCart(uid);

    return Results.Json(
        new
        {
          data
        }
    );
  }

  [HttpDelete("user/cart")]
  [Authorize(Roles = UserRole.DEFAULT)]
  public async Task<IResult> deleteCart([FromBody] RemoveProductFromCartBody body)
  {
    var uid = JWT.useToken(HttpContext);
    System.Console.WriteLine(body.PhoneId);
    System.Console.WriteLine(body.Count);
    var data = await _userService.deleteProductFromCart(uid, body);

    return Results.Json(
        new
        {
          message = data
        }
    );
  }
}
