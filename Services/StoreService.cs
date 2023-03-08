using System.Net;
using Microsoft.EntityFrameworkCore;
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

      await db.SaveChangesAsync();

      String script = $"""
        INSERT _{nameof(Storetouser)}(A, B)
          VALUES ({newStore.Id}, {user.Id});
      """;

      db.Database.ExecuteSqlRaw(script);

      db.SaveChanges();

      return newStore;
    });
  }
}
