using System.Net;
using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PhoneShop.Interface;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;
using PhoneShop.Prisma;

namespace PhoneShop.Service;

class GetStoreData
{
  public int Id { get; set; }
  public String Uid { get; set; }
  public String Name { get; set; }
  public String Location { get; set; }
  public String Group { get; set; }
  public String? PhoneNumber { get; set; }

  public Storetouser? Storetouser { get; set; }
  public List<dynamic>? Managers { get; set; }

  public List<Phone>? Phones { get; set; }
}



public class StoreService
{
  public StoreService() { }// constructer

  public async Task<Store> createStore(String userId, CreateStoreBody body)//Task<Store> là kiểu dữ liệu trả về dùng với async..await

  {
    return await PrismaExtension.runTransaction(async db =>//đảm bảo rằng tất cả các thao tác trên cơ sở dữ liệu sẽ được thực hiện trong một giao dịch (transaction).
    {
      var newStore = new Store()
      {
        Name = body.Name,
        Location = body.Location,
        Group = body.Group,
        PhoneNumber = body.PhoneNumber
      };

      var user = await db.Users.Where(user => user.Uid == userId).FirstAsync();

      if (user == null)
      {
        throw new HttpException("User not found", HttpStatusCode.NotFound);
      }

      await db.Stores.AddAsync(newStore);

      db.SaveChanges();

      await db.Database.addRelationship<Storetouser>(db, newStore.Id, user.Id);

      db.SaveChanges();

      return newStore;
    });
  }

  public async Task<dynamic?> getUserStores(string userUid)
  {
    return await PrismaExtension.runTask(async db =>
    {
      var userData = await db.Users.Where(x => x.Uid == userUid).FirstAsync();

      if (userData != null)
      {
        var relationships = await db.Storetousers.Where(x => x.B == userData.Id).ToListAsync();

        if (relationships != null)
        {
          return relationships.Select(stu => db.Stores.Where(x => x.Id == stu.A).Select(s => new
          {
            Id = s.Id,
            Uid = s.Uid,
            Name = s.Name,
            Location = s.Location,
            Group = s.Group,
            productCount = s.Phones.Count,
            updateAt = s.UpdateAt,
            createAt = s.CreateAt
          }).First()).ToList().OrderByDescending(s => s.Id);
        }
      }

      return null;
    });
  }

  public async Task<object?> getStore(string uid)
  {
    return await PrismaExtension.runTask(async db => await _getStore(db, uid));
  }

  public async Task<Store?> updateStore(String userId, String storeId, CreateStoreBody body)//Task<Store> là kiểu dữ liệu trả về dùng với async..await

  {
    return await PrismaExtension.runTransaction(async db =>//đảm bảo rằng tất cả các thao tác trên cơ sở dữ liệu sẽ được thực hiện trong một giao dịch (transaction).
    {
      var storeUser = await db.Storetousers.Include(x => x.ANavigation).Include(x => x.BNavigation).Where(stu => stu.ANavigation.Uid == storeId).FirstAsync();

      if (storeUser == null)
      {
        throw new HttpException("store not found", HttpStatusCode.NotFound);
      }

      if (storeUser.BNavigation.Uid != userId)
      {
        throw new HttpException("you not have permission to change this", HttpStatusCode.Forbidden);
      }

      var storeData = db.Stores.Where(x => x.Uid == storeId).First();

      if (body.Name != null)
      {
        storeData.Name = body.Name;
      }

      if (body.Location != null)
      {
        storeData.Location = body.Location;
      }

      if (body.Group != null)
      {
        storeData.Group = body.Group;
      }

      if (body.PhoneNumber != null)
      {
        storeData.PhoneNumber = body.PhoneNumber;
      }

      db.SaveChanges();

      return storeData;
    });
  }

  private async Task<GetStoreData?> _getStore(PrismaClient db, String uid)
  {
    var storeData = await db.Stores.Where(st => st.Uid == uid)
        .Join(db.Storetousers, store => store.Id, storeUser => storeUser.A,
        (store, storeUser) => new GetStoreData
        {
          Id = 0,
          Uid = store.Uid,
          Name = store.Name,
          Location = store.Location,
          Group = store.Group,
          PhoneNumber = store.PhoneNumber,
          Storetouser = storeUser,
        }).FirstAsync();

    if (storeData == null)
    {
      throw new HttpException("Store not found", HttpStatusCode.NotFound);
    }

    storeData.Phones = await db.Phones.Where(x => x.StoreId == storeData.Uid).ToListAsync();

    if (storeData.Storetouser != null)
    {
      storeData.Managers = await db.Users.Where(stu => stu.Id == storeData.Storetouser.B)
            .Select(u => new { Uid = u.Uid, Name = u.Name, Image = u.Image, Email = u.Email }).ToListAsync<dynamic>();
      storeData.Storetouser = null;
    }

    return storeData;
  }

  public async Task<string> deleteStore(String userId, String storeId)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var storeUser = await db.Storetousers
        .Include(x => x.ANavigation)
        .Include(x => x.BNavigation)
        .Where(stu => stu.ANavigation.Uid == storeId).FirstAsync();

      if (storeUser == null)
      {
        throw new HttpException("store not found", HttpStatusCode.NotFound);
      }

      if (storeUser.BNavigation.Uid != userId)
      {
        throw new HttpException("you not have permission to change this", HttpStatusCode.Forbidden);
      }

      db.Stores.Remove(storeUser.ANavigation);

      db.SaveChanges();

      return "success to remove store";
    });
  }

  public async Task<object?> createPhone(String userId, CreatePhoneBody phoneBody)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var storeId = phoneBody.StoreId;
      var storeUser = await db.Storetousers
        .Include(x => x.ANavigation)
        .Include(x => x.BNavigation)
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
        Phoneoffers = phoneBody.Phoneoffers!.Select(offer => new Phoneoffer
        {
          Price = offer.Price,
          Count = offer.Count,
          Color = offer.Color,
          Storage = offer.Storage,
        }).ToList()
      };

      await db.Phones.AddAsync(newPhone);

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
}
