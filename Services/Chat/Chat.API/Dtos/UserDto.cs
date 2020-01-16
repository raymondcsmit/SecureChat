using System.ComponentModel.DataAnnotations;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;
using Helpers.Specifications.Attributes;

namespace Chat.API.Dtos
{
    public class UserDto:  IMapTo<User>, IMapFrom<User>
    {
        [Searchable, Sortable]
        public string Id { get; set; }

        [MinLength(3), MaxLength(20)]
        [Searchable, Sortable]
        public string UserName { get; set; }

        [EmailAddress]
        [Searchable, Sortable]
        public string Email { get; set; }

        public ProfileDto Profile { get; set; }
    }
}
