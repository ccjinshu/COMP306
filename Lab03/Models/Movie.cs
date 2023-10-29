using System;
using System.Collections.Generic;

namespace Lab03.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Genre { get; set; }

    public string? Director { get; set; }

    public DateTime? ReleaseTime { get; set; }

    public decimal? Rating { get; set; }

    public string? CoverImage { get; set; }

    public string? FileKey { get; set; }

    public string? FileUrl { get; set; }
}
