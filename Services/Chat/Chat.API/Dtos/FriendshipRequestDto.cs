using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Dtos
{
    public class FriendshipRequestDto
    {
        public UserDto RequesterDto { get; set; }

        public UserDto RequesteeDto { get; set; }
    }
}
