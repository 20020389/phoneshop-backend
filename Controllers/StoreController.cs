
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Interface;
using PhoneShop.Middleware;
using PhoneShop.Service;

namespace PhoneShop.Controllers;

[HttpException]
[ApiController]
[Route("api")]
public class StoreController : Controller
{
  private readonly StoreService _storeService;
  public StoreController(StoreService storeService)
  {
    _storeService = storeService;
  }

  [HttpPost("store")]
  [Authorize]
  public async Task<IResult> createStore([FromBody] CreateStoreBody body)
  {

    var identity = HttpContext.User.Identity as ClaimsIdentity;
    if (identity != null)
    {
      var userClaims = identity.Claims;
      var uid = userClaims.FirstOrDefault(claim => claim.Type == "uid")?.Value ?? "null";
      return Results.Json(new
      {
        data = await _storeService.createStore(uid, body)
      });
    }
    return Results.Unauthorized();

  }
}