using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Lib;
using PhoneShop.Middleware;
using PhoneShop.Model;
using PhoneShop.Prisma;
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

    [HttpPost("register")]
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
            new Claim(ClaimTypes.Email, body.Email),
            new Claim(ClaimTypes.Role, UserRole.DEFAULT)
        };

        return Results.Json(
            new { data = newUser, token = $"Bearer {_jwt.generateToken(claims)}" }
        );
    }

    [HttpGet("user")]
    [Authorize]
    public IActionResult test()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var userClaims = identity.Claims;
            var email = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            var role = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
            return Ok(new { email, role });
        }
        return Ok(new { status = "Okay" });
    }
}
