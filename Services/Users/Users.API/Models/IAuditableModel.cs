using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Models
{
    public interface IAuditableModel
    {
        DateTime CreatedAt { get; set; }
        DateTime LastModified { get; set; }
    }
}
