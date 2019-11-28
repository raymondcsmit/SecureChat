using System;

namespace Users.API.Models
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset ModifiedAt { get; set; }
    }
}
