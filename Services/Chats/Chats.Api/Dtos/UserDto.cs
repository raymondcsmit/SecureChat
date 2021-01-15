using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chats.Domain.AggregateModel;
using Helpers.Mapping;

namespace Chats.Api.Dtos
{
    public class UserDto : IMapFrom<User>
    {
        public string Id { get; set; }

        public string UserName { get; set; }
    }
}
