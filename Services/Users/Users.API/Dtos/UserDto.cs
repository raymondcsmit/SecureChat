using System.ComponentModel.DataAnnotations;
using Users.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;
using Users.API.Application.Specifications.Attributes;

namespace Users.API.Dtos
{
    public class UserDto:  IMapTo<User>, IMapFrom<User>
    {
        private ProfileDto _profile;

        [Required]
        [Searchable, Sortable]
        public string Id { get; set; }

        [Required, MinLength(3), MaxLength(20)]
        [Searchable, Sortable]
        public string UserName { get; set; }

        [Required, EmailAddress]
        [Searchable, Sortable]
        public string Email { get; set; }

        public ProfileDto Profile
        {
            get { return _profile; }
            set { _profile = IsProfileEmpty(value) ? null : value; }
        }

        public UserDto()
        {

        }

        public UserDto(string id, string userName, string email, ProfileDto profile)
        {
            Id = id;
            UserName = userName;
            Email = email;
            Profile = profile;
        }

        public static UserDto ValidationUser => new UserDto("1", "test", "test@test.com", new ProfileDto { Age = 20 });

        private static bool IsProfileEmpty(ProfileDto profile)
        {
            return profile.Age == null && profile.Location == null && profile.Sex == null;
        }
    }
}
