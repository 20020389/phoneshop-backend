using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Interface;
using PhoneShop.Lib;
using PhoneShop.Middleware;
using PhoneShop.Service;

namespace PhoneShop.Controllers;

[HttpException]
[ApiController]
[Route("api")]
public class StoreController : Controller
{
  private readonly StoreService _storeService;
  public StoreController(StoreService storeService) //contructor
  {
    _storeService = storeService;
  }

  [HttpGet("user/stores")]
  [Authorize]
  public async Task<IResult> getStores()
  {
    var uid = JWT.useToken(HttpContext);
    return Results.Json(new
    {
      data = (await _storeService.getUserStores(uid))
    });
  }

  [HttpGet("store/id/{storeId}")]
  public async Task<IResult> getStoreData(String storeId)
  {
    return Results.Json(new
    {
      data = (await _storeService.getStore(storeId))
    });
  }

  [HttpPost("store")]
  [Authorize]
  public async Task<IResult> createStore([FromBody] CreateStoreBody body)
  {
    var uid = JWT.useToken(HttpContext);
    return Results.Json(new
    {
      data = (await _storeService.createStore(uid, body))
    });
  }

  [HttpPut("store/id/{storeId}")]
  [Authorize]
  public async Task<IResult> updateStore([FromBody] CreateStoreBody body, String storeId)
  {
    var uid = JWT.useToken(HttpContext);
    return Results.Json(new
    {
      data = (await _storeService.updateStore(uid, storeId, body))
    });
  }

  [HttpDelete("store/id/{storeId}")]
  [Authorize]
  public async Task<IResult> deleteStore(String storeId)
  {
    var uid = JWT.useToken(HttpContext);
    return Results.Json(new
    {
      data = (await _storeService.deleteStore(uid, storeId))
    });
  }

  [HttpPost("store/id/{storeId}/phones")]
  [Authorize]
  public async Task<IResult> addProduct([FromBody] CreatePhoneBody body, String storeId)
  {
    var uid = JWT.useToken(HttpContext);
    body.StoreId = storeId;
    var data = (await _storeService.createPhone(uid, body));

    return Results.Json(new
    {
      data
    }
    );
  }

}