using System;
using Microsoft.AspNetCore.Identity;

namespace Users.API.Models
{
    public class User : IdentityUser, IAuditable
    {
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
