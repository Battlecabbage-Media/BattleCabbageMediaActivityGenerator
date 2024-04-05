using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class Purchase
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public DateTime TransactionCreatedOn { get; set; }

    public Guid PurchaseLocationId { get; set; }

    public virtual ICollection<UserCreditCard> PaymentCards { get; set; } = new List<UserCreditCard>();

    public virtual ICollection<PurchaseLineItem> PurchaseLineItems { get; set; } = new List<PurchaseLineItem>();

    public virtual Kiosk PurchaseLocation { get; set; } = null!;

    public virtual ICollection<User> PurchasingUsers { get; set; } = new List<User>();
}
