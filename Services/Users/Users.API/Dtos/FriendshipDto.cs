using System;
using Users.API.Application.Specifications.Attributes;

namespace Users.API.Dtos
{
    public class FriendshipDto
    {
        [Searchable, Sortable]
        public string Id { get; set; }

        [Searchable("User1Id"), Sortable("User1Id")]
        public UserDto User1 { get; set; }

        [Searchable("User2Id"), Sortable("User2Id")]
        public UserDto User2 { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}
