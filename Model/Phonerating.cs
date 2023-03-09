using System;
using System.Collections.Generic;

namespace PhoneShop.Model;

public partial class Phonerating
{
    public int Id { get; set; }

    public string Uid { get; set; } = null!;

    public double RatingValue { get; set; }

    public string PhoneId { get; set; } = null!;

    public int? UserId { get; set; }

    public virtual Phone Phone { get; set; } = null!;

    public virtual User? User { get; set; }
}
