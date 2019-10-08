using System;
using Microsoft.AspNetCore.Identity;

namespace Users.API.Models
{
    public class User : IdentityUser, IAuditableModel
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
