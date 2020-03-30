using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;
using Helpers.Specifications.Attributes;

namespace Chat.API.Dtos
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
