namespace PhoneShop.Lib;

public class Util
{
  public static async Task<T?> useMemo<T>(Func<Task<T>> callback)
  {
    try
    {
      return await callback();
    }
    catch
    {
      return default(T);
    }
  }
}