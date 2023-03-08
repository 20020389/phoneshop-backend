
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Interface;
using PhoneShop.Middleware;
using PhoneShop.Service;

namespace PhoneShop.Controllers;

[HttpException]
[ApiController]
[Route("api/store")]
public class StoreController : Controller
{
  private readonly StoreService _storeService;
  public StoreController(StoreService storeService) //contructor
  {
    _storeService = storeService;
  }

  [HttpGet("userstores")]
  [Authorize]
  public async Task<IResult> getStores()
  {
    var identity = HttpContext.User.Identity as ClaimsIdentity;
    if (identity != null)
    {
      var userClaims = identity.Claims;
      var uid = userClaims.FirstOrDefault(claim => claim.Type == "uid")?.Value ?? "null";
      return Results.Json(new
      {
        data = (await _storeService.getUserStores(uid))
      });
    }
    return Results.Unauthorized();
  }

  [HttpGet("id/{storeId}")]
  public async Task<IResult> getStoreData(String storeId)
  {
    return Results.Json(new
    {
      data = (await _storeService.getStore(storeId))
    });
  }

  [HttpPost]
  [Authorize]
  public async Task<IResult> createStore([FromBody] CreateStoreBody body)
  {

    var identity = HttpContext.User.Identity as ClaimsIdentity;//lấy thông tin người dùng đã xác thực từ httpContext
    if (identity != null)
    {
      var userClaims = identity.Claims;//lấy các thông tin chi tiết của người dùng từ đối tượng ClaimsIdentity đã được trích xuất từ HttpContext
      var uid = userClaims.FirstOrDefault(claim => claim.Type == "uid")?.Value ?? "null";
      //lấy giá trị của thuộc tính uid trong các thông tin chi tiết của người dùng. Nếu không tìm thấy giá trị này, một chuỗi rỗng sẽ được trả về.
      return Results.Json(new
      {
        data = (await _storeService.createStore(uid, body))
      });
    }
    return Results.Unauthorized();//trả về mã trạng thái HTTP 401 Unauthorized nếu người dùng chưa được xác thực.

  }
}