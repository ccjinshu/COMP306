using System;
using System.Collections.Generic;

namespace WebAppAccess2RDS.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Program { get; set; }

    public virtual Login? Login { get; set; }

    public virtual ICollection<Course> CourseCodes { get; set; } = new List<Course>();
}
