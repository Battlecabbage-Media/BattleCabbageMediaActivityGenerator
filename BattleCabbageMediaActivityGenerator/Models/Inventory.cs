using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class Inventory
{
    public int InventoryId { get; set; }

    public string ItemDescription { get; set; } = null!;

    public decimal CurrentPrice { get; set; }

    public virtual ICollection<PurchaseLineItem> PurchaseLineItems { get; set; } = new List<PurchaseLineItem>();
}
