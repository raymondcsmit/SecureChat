using System;

namespace Chat.Domain.SeedWork
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset ModifiedAt { get; }
    }
}
