using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class UserCreditCard
{
    public int CreditCardId { get; set; }

    public int UserId { get; set; }

    public string CreditCardNumber { get; set; } = null!;

    public DateTime CreditCardExpiration { get; set; }

    public bool Default { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual User User { get; set; } = null!;
}
