using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Users.API.Models
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset ModifiedAt { get; set; }
    }
}
