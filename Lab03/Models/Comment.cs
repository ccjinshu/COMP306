using System;
using System.Collections.Generic;

namespace Lab03.Models;

public partial class Comment
{
    public string Id { get; set; }
    public string MovieId { get; set; }

    public string Content { get; set; } = null!;

    public string  UserId { get; set; }

    public DateTime? UpdateTime { get; set; }

    public double Rating { get; set; }

    
    //constructor
    public Comment()
    {
        Id = Guid.NewGuid().ToString();
    }

}
 