using System;

namespace Chats.Domain.SeedWork
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset ModifiedAt { get; set; }
    }
}
