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

namespace PhoneShop.Service;

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
      var storeDatas = await db.Users.Where(x => x.Uid == userUid)
        .GroupJoin(db.Storetousers, x => x.Id, y => y.B, (user, storeToUser) => new
        {
          storeToUser
        }).Select(res => db.Stores.GroupJoin(res.storeToUser, x => x.Id, y => y.A, (store, _) => new
        {
          Uid = store.Uid,
          Name = store.Name,
          Location = store.Location,
          Group = store.Group,
          PhoneNumber = store.PhoneNumber,
          Transactions = store.Transactions,
          phoneCount = db.Phonetostores.Where(x => x.B == store.Id).Count()
        }).ToList()).FirstAsync();

      return storeDatas;
    });
  }

  public async Task<object?> getStore(string uid)
  {
    return await PrismaExtension.runTask(async db =>
    {
      var storeData = await db.Stores.Where(st => st.Uid == uid)
        .Join(db.Storetousers, store => store.Id, storeUser => storeUser.A,
        (store, storeUser) => new
        {
          Id = 0,
          Uid = store.Uid,
          Name = store.Name,
          Location = store.Location,
          Group = store.Group,
          PhoneNumber = store.PhoneNumber,
          Managers = storeUser
        }).FirstAsync();

      if (storeData == null)
      {
        throw new HttpException("Store not found", HttpStatusCode.NotFound);
      }

      if (storeData.Managers != null)
      {
        var Managers = await db.Users.Where(stu => stu.Id == storeData.Managers.B)
              .Select(u => new { uid = u.Uid }).ToListAsync();

        return storeData.merge(new
        {
          Managers,
        });
      }

      return storeData;
    });
  }
}
