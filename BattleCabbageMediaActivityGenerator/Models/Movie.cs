using System;
using System.Collections.Generic;

namespace BattleCabbageMediaActivityGenerator.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string ExternalId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Tagline { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string MpaaRating { get; set; } = null!;

    public decimal PopularityScore { get; set; }

    public int GenreId { get; set; }

    public string? PosterUrl { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<UserReview> UserReviews { get; set; } = new List<UserReview>();
}
