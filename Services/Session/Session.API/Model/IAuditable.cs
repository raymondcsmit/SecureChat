using System;

namespace Session.API.Models
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset ModifiedAt { get; set; }
    }
}
