using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class UserSubscriptionStatus
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int RenewalDay { get; set; }

    public bool Active { get; set; }

    public Guid? MostRecentSubscriptionPurchaseId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual PurchaseLineItem? MostRecentSubscriptionPurchase { get; set; }
}
