
using System.Data;
using System.Data.Common;
using PhoneShop.Prisma;

namespace Microsoft.EntityFrameworkCore.Infrastructure;

public static class DatabaseExtension
{

  public static async Task<bool> addRelationship<T>(this DatabaseFacade _db, PrismaClient db, int AID, int BID)
  {
    try
    {
      String script = $"""
        INSERT _{typeof(T).Name.ToLower()}(A, B)
          VALUES ({AID}, {BID});
      """;

      await db.Database.ExecuteSqlRawAsync(script);

      return true;
    }
    catch (Exception e)
    {
      System.Console.WriteLine(e);
      return false;
    }
  }

  public static async Task<bool> addRelationship<T>(this DatabaseFacade _db, PrismaClient db, String AID, String BID)
  {
    try
    {
      String script = $"""
        INSERT _{typeof(T).Name.ToLower()}(A, B)
          VALUES ({AID}, {BID});
      """;

      await db.Database.ExecuteSqlRawAsync(script);

      return true;
    }
    catch (Exception e)
    {
      System.Console.WriteLine(e);
      return false;
    }
  }

  // public static async Task<object> ExecuteScalar(this DatabaseFacade database,
  //       string sql, List<DbParameter> parameters,
  //       CommandType commandType = CommandType.Text,
  //       int? commandTimeOutInSeconds = null)
  // {
  //   Object value;
  //   using (var cmd = database.GetDbConnection().CreateCommand())
  //   {
  //     if (cmd.Connection.State != ConnectionState.Open)
  //     {
  //       cmd.Connection.Open();
  //     }
  //     cmd.CommandText = sql;
  //     cmd.CommandType = commandType;
  //     if (commandTimeOutInSeconds != null)
  //     {
  //       cmd.CommandTimeout = (int)commandTimeOutInSeconds;
  //     }
  //     if (parameters != null)
  //     {
  //       cmd.Parameters.AddRange(parameters.ToArray());
  //     }
  //     value = cmd.ExecuteScalarAsync();
  //   }
  //   return value;
  // }
}