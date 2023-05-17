using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
  private readonly PhoneService _phoneService;
  public StoreController(StoreService storeService, PhoneService phoneService) //contructor
  {
    _storeService = storeService;
    _phoneService = phoneService;
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

  [HttpGet("phones/id/{phoneId}")]
  public async Task<IResult> getProduct(String phoneId)
  {
    var uid = JWT.tryGetToken(HttpContext);
    var data = (await _phoneService.getPhone(phoneId, uid));

    return Results.Json(new
    {
      data
    });
  }

  [HttpPost("store/id/{storeId}/phones")]
  [Authorize]
  public async Task<IResult> addProduct([FromBody] CreatePhoneBody body, String storeId)
  {
    var uid = JWT.useToken(HttpContext);
    body.StoreId = storeId;
    var data = (await _phoneService.createPhone(uid, body));

    return Results.Json(new
    {
      data
    });
  }

  [HttpDelete("phone/id/{phoneId}")]
  [Authorize]
  public async Task<IResult> deleteProduct(String phoneId)
  {
    var uid = JWT.useToken(HttpContext);
    var data = (await _phoneService.deletePhones(uid, phoneId));

    return Results.Json(new
    {
      message = data
    });
  }

  [HttpGet("phones/newest")]
  public async Task<IResult> getNewestProducts()
  {
    var query = Request.Query;
    var page = 1;
    if (!query["page"].IsNullOrEmpty())
    {
      try
      {
        page = Int32.Parse(query["page"].ToString());
      }
      catch (System.Exception)
      {

      }
    }
    var data = (await _phoneService.getNewestPhone(new Pagination
    {
      page = page,
      limit = 30
    }));

    return Results.Json(new
    {
      data
    });
  }


  [HttpGet("store/id/{storeId}/phones")]
  [Authorize]
  public async Task<IResult> getProducts(String storeId)
  {
    var query = Request.Query;
    var uid = JWT.useToken(HttpContext);

    var page = 1;
    if (!query["page"].IsNullOrEmpty())
    {
      try
      {
        page = Int32.Parse(query["page"].ToString());
      }
      catch (System.Exception)
      {

      }
    }

    var data = (await _phoneService.getPhones(uid, storeId, new Pagination
    {
      page = page,
      limit = 20
    }));

    return Results.Json(new
    {
      data
    });
  }
}