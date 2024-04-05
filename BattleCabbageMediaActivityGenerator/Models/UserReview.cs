using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class UserReview
{
    public Guid Id { get; set; }

    public int MovieId { get; set; }

    public double UserReviewScore { get; set; }

    public string? UserReview1 { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid UserId { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
