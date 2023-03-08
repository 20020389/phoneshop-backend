using Microsoft.EntityFrameworkCore;
using PhoneShop.Prisma;

namespace PhoneShop.Lib.Extension;

public class PrismaExtension
{
  public static async Task<T> runTransaction<T>(Func<PrismaClient, Task<T>> callback)
  {
    using (var db = new PrismaClient())
    {
      using (var transaction = db.Database.BeginTransaction())
      {
        try
        {
          var data = await callback(db);
          transaction.Commit();
          return data;
        }
        catch (System.Exception e)
        {
          transaction.Rollback();
          if (e is DbUpdateException && e.InnerException != null)
          {
            throw new HttpException(e.InnerException.Message);
          }

          throw new HttpException(e.Message);
        }
      }
    }
  }

  public static T runTransaction<T>(Func<PrismaClient, T> callback)
  {
    using (var db = new PrismaClient())
    {
      using (var transaction = db.Database.BeginTransaction())
      {
        try
        {
          var data = callback(db);
          transaction.Commit();
          return data;
        }
        catch (System.Exception e)
        {
          transaction.Rollback();
          if (e is DbUpdateException && e.InnerException != null)
          {
            throw new HttpException(e.InnerException.Message);
          }

          throw new HttpException(e.Message);
        }
      }
    }
  }

  public static async Task runTransaction(Func<PrismaClient, Task> callback)
  {
    using (var db = new PrismaClient())
    {
      using (var transaction = db.Database.BeginTransaction())
      {
        try
        {
          await callback(db);
          transaction.Commit();
        }
        catch (System.Exception e)
        {
          transaction.Rollback();
          if (e is DbUpdateException && e.InnerException != null)
          {
            throw new HttpException(e.InnerException.Message);
          }

          throw new HttpException(e.Message);
        }
      }
    }
  }

  public static void runTransaction(Action<PrismaClient> callback)
  {
    using (var db = new PrismaClient())
    {
      using (var transaction = db.Database.BeginTransaction())
      {
        try
        {
          callback(db);
          transaction.Commit();
        }
        catch (System.Exception e)
        {
          transaction.Rollback();
          if (e is DbUpdateException && e.InnerException != null)
          {
            throw new HttpException(e.InnerException.Message);
          }

          throw new HttpException(e.Message);
        }
      }
    }
  }

  public static async Task<T> runTask<T>(Func<PrismaClient, Task<T>> callback)
  {
    using (var db = new PrismaClient())
    {
      try
      {
        return await callback(db);
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

  public static async Task runTask(Func<PrismaClient, Task> callback)
  {
    using (var db = new PrismaClient())
    {
      try
      {
        await callback(db);
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

  public static void runTask(Action<PrismaClient> callback)
  {
    using (var db = new PrismaClient())
    {
      try
      {
        callback(db);
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