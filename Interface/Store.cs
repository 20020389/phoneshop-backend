using PhoneShop.Model;

namespace PhoneShop.Interface;

public class CreateStoreBody
{
  public string Name { get; set; } = null!;

  public string Location { get; set; } = null!;

  public string Group { get; set; } = null!;

  public string? PhoneNumber { get; set; }
}

public class StoreModel : Store
{
  public List<PhoneModel> Products { get; set; }
  public List<User> Managers { get; set; }
  public List<TransactionModel> Transactions { get; set; }
}

public class PhoneModel : Phone
{
  public List<StoreModel> Stores { get; set; }
}

public class TransactionModel : Transaction
{
  public List<StoreModel> Stores { get; set; }
}
