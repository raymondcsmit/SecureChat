using System;
using Users.API.Application.Specifications.Attributes;

namespace Users.API.Dtos
{
    public class FriendshipRequestDto
    {
        [Searchable, Sortable]
        public string Id { get; set; }

        [Searchable]
        public string Outcome { get; set; }

        [Searchable("RequesterId"), Sortable("RequesterId")]
        public UserDto Requester { get; set; }

        [Searchable("RequesteeId"), Sortable("RequesteeId")]
        public UserDto Requestee { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset ModifiedAt { get; set; }
    }
}
