using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class Return
{
    public Guid Id { get; set; }

    public Guid RentalId { get; set; }

    public DateTime ReturnDate { get; set; }

    public int LateDays { get; set; }

    public Guid? LateChargeLineItemId { get; set; }

    public virtual PurchaseLineItem LateChargeLineItem { get; set; } = null!;

    public virtual Rental Rental { get; set; } = null!;
}
