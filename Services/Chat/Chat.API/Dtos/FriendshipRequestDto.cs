using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;

namespace Chat.API.Dtos
{
    public class FriendshipRequestDto : IMapFrom<FriendshipRequest>
    {
        public string Id { get; set; }

        public UserDto Requester { get; set; }

        public UserDto Requestee { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
