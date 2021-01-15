using System;

namespace Messaging.Dtos
{
    public class FriendshipDto
    {
        public string Id { get; set; }

        public UserDto User1 { get; set; }

        public UserDto User2 { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}
