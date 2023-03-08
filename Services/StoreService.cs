
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
  public StoreService() { }

  public async Task<Store> createStore(String userId, CreateStoreBody body)
  {
    return await PrismaExtension.runTransaction(async db =>
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
      var storeDatas = await db.Database.SqlQueryRaw<StoreQuery>($"""
          SELECT s.uid, s.name, s.location, s.group, s.phoneNumber, s.soluong as productCount
            FROM user AS u
            JOIN (SELECT s2.*, _storetouser.B
               FROM _storetouser
                  JOIN (SELECT st.*, COUNT(p.A) AS soluong
                    FROM store AS st
                      JOIN _phonetostore p ON st.id = p.B
                            GROUP BY p.B) s2
                             ON A = s2.id) s ON u.id = s.B
            WHERE u.id = 1;
      """).AsNoTracking().ToListAsync();

      System.Console.WriteLine(storeDatas.ToArray());

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
