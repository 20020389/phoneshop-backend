using Microsoft.EntityFrameworkCore;
using PhoneShop.Prisma;

namespace PhoneShop.Lib.Extension;

public class PrismaExtension
{
  public static async Task<T> runTransaction<T>(Func<PrismaClient, Task<T>> callback)
  {
    using (var db = new PrismaClient())
    {
      try
      {
        var data = await callback(db);
        db.SaveChanges();
        return data;
      }
      catch (System.Exception e)
      {
        if (e is DbUpdateException && e.InnerException != null)
        {
          throw new HttpException(e.InnerException.Message);
        }

        throw new HttpException(e.Message);
      }
    }
  }

  public static async Task runTransaction(Func<PrismaClient, Task> callback)
  {
    using (var db = new PrismaClient())
    {
      try
      {
        await callback(db);
        db.SaveChanges();
      }
      catch (System.Exception e)
      {
        if (e is DbUpdateException && e.InnerException != null)
        {
          throw new HttpException(e.InnerException.Message);
        }

        throw new HttpException(e.Message);
      }
    }
  }

  public static void runTransaction(Action<PrismaClient> callback)
  {
    using (var db = new PrismaClient())
    {
      try
      {
        callback(db);
        db.SaveChanges();
      }
      catch (System.Exception e)
      {
        if (e is DbUpdateException && e.InnerException != null)
        {
          throw new HttpException(e.InnerException.Message);
        }

        throw new HttpException(e.Message);
      }
    }
  }
}