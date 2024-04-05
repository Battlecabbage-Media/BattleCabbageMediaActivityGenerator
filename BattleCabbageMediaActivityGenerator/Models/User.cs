using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateTime MemberSince { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

    public virtual ICollection<UserCreditCard> UserCreditCards { get; set; } = new List<UserCreditCard>();

    public virtual ICollection<UserReview> UserReviews { get; set; } = new List<UserReview>();

    public virtual UserSubscriptionStatus UserSubscriptionStatus { get; set; } = null!;
}
