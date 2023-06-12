using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhoneShop.Model;

public partial class Transaction
{
  [JsonIgnore]
  public int Id { get; set; }

  public string Uid { get; set; } = null!;

  public string Status { get; set; } = null!;

  public string? UserId { get; set; }

  public DateTime UpdateAt { get; set; }

  public DateTime CreateAt { get; set; }

  public int StoreId { get; set; }

  [JsonIgnore]
  public virtual Store? Store { get; set; } = null!;

  public virtual ICollection<Stringtemplate> Stringtemplates { get; set; } = new List<Stringtemplate>();

  [JsonIgnore]
  public virtual User? User { get; set; }
}
