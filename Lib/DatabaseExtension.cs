using Microsoft.EntityFrameworkCore;
using PhoneShop.Lib;

namespace PhoneShop.Prisma;

static class DatabaseExtension
{
  public static async Task runTransaction(this PrismaClient db, Func<PrismaClient, Task> callback)
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

  public static void runTransaction(this PrismaClient db, Action<PrismaClient> callback)
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