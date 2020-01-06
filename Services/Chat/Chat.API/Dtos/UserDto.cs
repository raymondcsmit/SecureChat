using System.ComponentModel.DataAnnotations;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;

namespace Chat.API.Dtos
{
    public class UserDto:  IMapTo<User>, IMapFrom<User>
    {
        public string Id { get; set; }

        [MinLength(3), MaxLength(20)]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public ProfileDto Profile { get; set; }
    }
}
