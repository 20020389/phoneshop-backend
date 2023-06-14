using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PhoneShop.Interface;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;
using PhoneShop.Prisma;

namespace PhoneShop.Service;

class GetStoreData
{
  public int Id { get; set; }
  public String? Uid { get; set; }
  public String? Name { get; set; }
  public String? Location { get; set; }
  public String? Group { get; set; }
  public String? PhoneNumber { get; set; }

  public Storetouser? Storetouser { get; set; }
  public List<dynamic>? Managers { get; set; }

  public List<String>? Phones { get; set; }
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

  public async Task<object> getStoreTransactions(string userId, string storeId)
  {
    return await PrismaExtension.runTransaction(async db =>
      {
        var storeUser = await db.Storetousers.Include(x => x.ANavigation).Include(x => x.BNavigation).Where(stu => stu.ANavigation.Uid == storeId).FirstAsync();

        if (storeUser == null)
        {
          throw new HttpException("store not found", HttpStatusCode.NotFound);
        }

        if (storeUser.BNavigation.Uid != userId)
        {
          throw new HttpException("you not have permission to do this", HttpStatusCode.Forbidden);
        }

        var transactions = await db.Transactions.Include(t => t.User).Include(t => t.Stringtemplates).Where(t => t.StoreId == storeUser.A).Select(t => new
        {
          Id = t.Id,
          Uid = t.Uid,
          Status = t.Status,
          UserId = t.UserId,
          UpdateAt = t.UpdateAt,
          CreateAt = t.CreateAt,
          Store = t.Store,
          User = t.User,
          products = t.Stringtemplates.Select(st => db.Phoneoffers
            .Include(po => po.Phone).Select(po => new
            {
              Uid = po.Uid,
              Price = po.Price,
              Count = po.Count,
              Color = po.Color,
              Storage = po.Storage,
              Name = po.Phone.Name
            }).Where(po => po.Uid == st.Value).First()).ToList()
        }).OrderByDescending(t => t.CreateAt).ToListAsync(); ;

        // TODO
        return transactions;
      });
  }

  public async Task<object> getUserTransactions(string userId)
  {
    return await PrismaExtension.runTransaction(async db =>
      {
        var transactions = await db.Transactions.Include(t => t.Stringtemplates).Include(t => t.Store).Select(t => new
        {
          Id = t.Id,
          Uid = t.Uid,
          Status = t.Status,
          UserId = t.UserId,
          UpdateAt = t.UpdateAt,
          CreateAt = t.CreateAt,
          Store = t.Store,
          products = t.Stringtemplates.Select(st => db.Phoneoffers
            .Include(po => po.Phone).Select(po => new
            {
              Uid = po.Uid,
              Price = po.Price,
              Count = po.Count,
              Color = po.Color,
              Storage = po.Storage,
              Name = po.Phone.Name
            }).Where(po => po.Uid == st.Value).First()).ToList()
        }).OrderByDescending(t => t.CreateAt).Where(t => t.UserId == userId).ToListAsync();

        // TODO
        return transactions;
      });
  }

  public async Task<object> confirmTransaction(string userId, string storeId, ConfirmTransaction confirmData)
  {
    return await PrismaExtension.runTransaction(async db =>
      {
        var storeUser = await db.Storetousers.Include(x => x.ANavigation).Include(x => x.BNavigation).Where(stu => stu.ANavigation.Uid == storeId).FirstAsync();

        if (storeUser == null)
        {
          throw new HttpException("store not found", HttpStatusCode.NotFound);
        }

        if (storeUser.BNavigation.Uid != userId)
        {
          throw new HttpException("you not have permission to do this", HttpStatusCode.Forbidden);
        }

        var transaction = await db.Transactions.Include(t => t.Stringtemplates).Where(t => t.StoreId == storeUser.A && t.Uid == confirmData.transactionId).FirstAsync();

        if (transaction == null)
        {
          throw new HttpException("transaction not found", HttpStatusCode.NotFound);
        }

        if (transaction.Status != TransactionStatus.PROCESSING)
        {
          throw new HttpException("transaction is confirmed", HttpStatusCode.NotModified);
        }

        var confirmStatus = confirmData.status.ToUpper();



        if (confirmStatus == TransactionStatus.SUCCESS)
        {
          foreach (var offerTransaction in transaction.Stringtemplates)
          {
            var offer = await db.Phoneoffers.Where(po => po.Uid == offerTransaction.Value).FirstAsync();
            if (offer == null)
            {
              throw new HttpException($"offer id {offerTransaction.Value} not found", HttpStatusCode.NotFound);
            }

            if (offer.Count <= 0)
            {
              throw new HttpException($"The product with id {offer.PhoneId} is out of stock", HttpStatusCode.NotFound);
            }

            offer.Count = offer.Count - 1;
          }

          transaction.Status = TransactionStatus.SUCCESS;
        }
        else
        {
          transaction.Status = TransactionStatus.REFUSE;
        }

        await db.SaveChangesAsync();

        // TODO
        return transaction;
      });
  }

  public async Task<object> deleteTransaction(string userId, String transactionId)
  {
    return await PrismaExtension.runTransaction(async db =>
      {
        var transaction = await db.Transactions.Where(t => t.Uid == transactionId).FirstAsync();

        if (transaction == null)
        {
          throw new HttpException("transaction not found", HttpStatusCode.NotFound);
        }

        db.Transactions.Remove(transaction);

        await db.SaveChangesAsync();

        return "success";
      });
  }

  public async Task<object?> createTransaction(String userId, String storeId, string offerId)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var user = await Util.useMemo(() => db.Users.Where(u => u.Uid == userId).FirstAsync());
      var store = await Util.useMemo(() => db.Stores.Where(s => s.Uid == storeId).FirstAsync());

      var offer = await Util.useMemo(() => db.Phoneoffers.Where(po => po.Uid == offerId).FirstAsync());

      if (user == null)
      {
        throw new HttpException("User not found", HttpStatusCode.NotFound);
      }

      if (store == null)
      {
        throw new HttpException("Store not found", HttpStatusCode.NotFound);
      }

      if (offer == null)
      {
        throw new HttpException("Offer not found", HttpStatusCode.NotFound);
      }

      if (user.Role == UserRole.STORE)
      {
        throw new HttpException("You are not consumer", HttpStatusCode.BadRequest);
      }
      var transaction = new Transaction()
      {
        StoreId = store.Id,
        UserId = user.Uid,
        Status = TransactionStatus.PROCESSING,
        Stringtemplates = new[] { new Stringtemplate() { Value = offer.Uid, } },
        CreateAt = DateTime.Now,
        UpdateAt = DateTime.Now,
      };

      try
      {
        await db.Transactions.AddAsync(transaction);
        await db.SaveChangesAsync();
      }
      catch (System.Exception e)
      {
        System.Console.WriteLine(e);
      }

      return transaction;
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

    storeData.Phones = await db.Phones.Where(x => x.StoreId == storeData.Uid).Select(p => p.Uid).ToListAsync();

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


}
