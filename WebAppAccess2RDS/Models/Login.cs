using System;
using System.Collections.Generic;

namespace WebAppAccess2RDS.Models;

public partial class Login
{
    public string LoginName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual Student LoginNameNavigation { get; set; } = null!;
}
