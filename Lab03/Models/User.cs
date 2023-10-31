using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab03.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;
    [NotMapped]
    public string Password { get; set; } = null!;
    

    //ConfirmPassword  is not a column in the database, but is used to confirm the password entered by the user when registering
    [Compare("Password")]
    [NotMapped]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = null!;

    [BindNever]
    public string PasswordHash { get; set; } = null!;



}
