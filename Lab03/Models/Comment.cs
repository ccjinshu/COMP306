using System;
using System.Collections.Generic;

namespace Lab03.Models;

public partial class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public int? UserId { get; set; }

    public DateTime? UpdateTime { get; set; }

    public decimal? Rating { get; set; }
}
