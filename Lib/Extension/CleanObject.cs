using System.Dynamic;
using System.Text.Json;

public static class ReClasser
{
  public static object? merge<T, V>(this T currentObj, V obj)
  {
    var t = currentObj!.GetType();
    var returnClass = new ExpandoObject() as IDictionary<string, object>;
    if (obj != null)
    {
      var v = obj.GetType();
      foreach (var pr in v.GetProperties())
      {
        var val = pr.GetValue(obj);
        returnClass[pr.Name.lowerFirst()] = val!;
      }
    }

    foreach (var pr in t.GetProperties())
    {
      var val = pr.GetValue(currentObj);
      if (val is string && string.IsNullOrWhiteSpace(val.ToString())) { }
      else if (val == null) { }
      else
      {
        if (returnClass.ContainsKey(pr.Name.lowerFirst()) != true)
        {
          returnClass[pr.Name.lowerFirst()] = val!;
        }
      }
    }

    return returnClass;
  }

  public static T? parse<T>(this object obj)
  {
    return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
  }

  public static String lowerFirst(this String str)
  {
    return char.ToLower(str[0]) + str.Substring(1);
  }
}