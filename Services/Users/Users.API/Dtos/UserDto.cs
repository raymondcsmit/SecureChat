using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers.Mapping;
using Users.API.Models;

namespace Users.API.Dtos
{
    public class UserDto : IMapFrom<User>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string CreatedAt { get; set; }
    }
}
