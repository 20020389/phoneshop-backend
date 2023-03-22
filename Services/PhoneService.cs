
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using PhoneShop.Interface;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;

public class PhoneService
{

  public async Task<object?> createPhone(String userId, CreatePhoneBody phoneBody)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var storeId = phoneBody.StoreId;
      var storeUser = await db.Storetousers.Include(x => x.ANavigation).Include(x => x.BNavigation)
        .Where(stu => stu.ANavigation.Uid == storeId).FirstAsync();

      if (storeUser == null)
      {
        throw new HttpException("store not found", HttpStatusCode.NotFound);
      }

      if (storeUser.BNavigation.Uid != userId)
      {
        throw new HttpException("you not have permission to change this", HttpStatusCode.Forbidden);
      }

      var newPhone = new Phone()
      {
        Name = phoneBody.Name,
        Images = phoneBody.Images,
        StoreId = storeId!,

      };
      await db.Phones.AddAsync(newPhone);

      db.SaveChanges();

      var Phoneoffers = phoneBody.Phoneoffers!.Select(offer => new Phoneoffer
      {
        Price = offer.Price,
        Count = offer.Count,
        Color = offer.Color,
        Storage = offer.Storage,
        PhoneId = newPhone.Uid
      }).ToList();

      await db.Phoneoffers.AddRangeAsync(Phoneoffers);



      db.SaveChanges();

      return new
      {
        Uid = newPhone.Uid,
        Name = newPhone.Name,
        Images = newPhone.Images,
        Tags = newPhone.Tags,
        Profile = newPhone.Profile,
        Description = newPhone.Description,
        Detail = newPhone.Detail,
        Rating = newPhone.Rating,
        StoreId = newPhone.StoreId,
        UpdateAt = newPhone.UpdateAt,
        CreateAt = newPhone.CreateAt,
        Phoneoffers = newPhone.Phoneoffers.Select(pof => new
        {
          Price = pof.Price,
          Count = pof.Count,
          Color = pof.Color,
          Storage = pof.Storage
        }),
      };
    });
  }

  public async Task<object> getPhones(String userId, String storeId, Pagination pagination)
  {
    return await PrismaExtension.runTask(async db =>
    {
      var storeUser = await db.Storetousers.Include(x => x.ANavigation).Include(x => x.BNavigation)
        .Where(stu => stu.ANavigation.Uid == storeId).FirstAsync();

      if (storeUser == null)
      {
        throw new HttpException("store not found", HttpStatusCode.NotFound);
      }

      if (storeUser.BNavigation.Uid != userId)
      {
        throw new HttpException("you not have permission to get data", HttpStatusCode.Forbidden);
      }

      var phones = await db.Phones.Where(p => p.StoreId == storeId).Select(p => new
      {
        Uid = p.Uid,
        Name = p.Name,
        Images = p.Images,
        Tags = p.Tags,
        Profile = p.Profile,
        Description = p.Description,
        Detail = p.Detail,
        Rating = p.Rating,
        StoreId = p.StoreId,
        UpdateAt = p.UpdateAt,
        CreateAt = p.CreateAt,
        Phoneoffers = p.Phoneoffers.Select(pof => new
        {
          Price = pof.Price,
          Count = pof.Count,
          Color = pof.Color,
          Storage = pof.Storage
        }),
      }).ToListAsync();

      return phones;
    });
  }
}