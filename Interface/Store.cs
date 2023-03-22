using PhoneShop.Model;

namespace PhoneShop.Interface;

public class CreateStoreBody
{
  public string Name { get; set; } = null!;

  public string Location { get; set; } = null!;

  public string Group { get; set; } = null!;

  public string? PhoneNumber { get; set; }
}

public class StoreGetQuery
{
  public String storeid { get; set; }
}

public class StoreModel : Store
{
  public List<PhoneModel>? Products { get; set; }
  public List<User>? Managers { get; set; }
  public List<TransactionModel>? Transactions { get; set; }

}

public class StoreQuery
{

  public string Uid { get; set; } = null!;

  public string Name { get; set; } = null!;

  public string Location { get; set; } = null!;

  public string Group { get; set; } = null!;

  public string? PhoneNumber { get; set; }

  public int? productCount { get; set; }
}

public class PhoneModel : Phone
{
  public List<StoreModel> Stores { get; set; }
}

public class TransactionModel : Transaction
{
  public List<StoreModel> Stores { get; set; }
}

public class CreatePhoneBody
{
  public string Name { get; set; } = null!;

  public string? Images { get; set; }

  public string? Tags { get; set; }

  public string? Profile { get; set; }

  public string? Description { get; set; }

  public string? Detail { get; set; }

  public double? Rating { get; set; }

  public string? StoreId { get; set; } = null!;

  public virtual ICollection<PhoneofferModel>? Phoneoffers { get; set; } = new List<PhoneofferModel>();
}

public class PhoneofferModel
{
  public int Price { get; set; }

  public int Count { get; set; }

  public string Color { get; set; } = null!;

  public string Storage { get; set; } = null!;
}

public class Pagination
{
  public int page;
  public int limit = 20;

  public int? total;
  public bool? hasNext;
  public bool? hasPrev;
}