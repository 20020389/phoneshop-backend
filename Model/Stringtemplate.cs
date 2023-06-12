using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PhoneShop.Model;

public partial class Stringtemplate
{
  public int Id { get; set; }

  public string Value { get; set; } = null!;

  [JsonIgnore]
  public int? TransactionId { get; set; }

  public int? CartId { get; set; }

  public virtual Cart? Cart { get; set; }

  [JsonIgnore]
  public virtual Transaction? Transaction { get; set; }
}
