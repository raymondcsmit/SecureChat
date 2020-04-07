using System;

namespace Users.Domain.SeedWork
{
    public interface IAuditable
    {
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset ModifiedAt { get; }
    }
}
