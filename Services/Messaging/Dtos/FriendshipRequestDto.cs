using System;

namespace Messaging.Dtos
{
    public class FriendshipRequestDto
    {
        public string Id { get; set; }

        public string Outcome { get; set; }

        public UserDto Requester { get; set; }

        public UserDto Requestee { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}
