using Account.API.Models;
using Helpers.Mapping;

namespace Account.API.Dtos
{
    public class UserDto : IMapFrom<User>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string CreatedAt { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
