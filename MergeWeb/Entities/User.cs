﻿using Microsoft.AspNetCore.Identity;

namespace MergeWeb.Entities;


public class User : IdentityUser
{
    public DateOnly? DateOfBirth { get; set; }
    public string? Nationality { get; set; }

}
