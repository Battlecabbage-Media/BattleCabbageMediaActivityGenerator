using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class PurchaseLineItem
{
    public int PurchaseLineItemId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public int PurchaseId { get; set; }

    public virtual Inventory Item { get; set; } = null!;

    public virtual Purchase Purchase { get; set; } = null!;

    public virtual Return? Return { get; set; }

    public virtual Rental? Rental { get; set; }

    public virtual UserSubscriptionStatus? UserSubscriptionStatus { get; set; }
}
