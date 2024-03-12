using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    public int MovieId { get; set; }

    public int UserId { get; set; }

    public DateTime RentalDate { get; set; }

    public DateTime ExpectedReturnDate { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual Return? Return { get; set; }

    public virtual PurchaseLineItem PurchaseLineItem { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
