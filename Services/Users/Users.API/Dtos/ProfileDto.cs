using System.ComponentModel.DataAnnotations;
using Users.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;

namespace Users.API.Dtos
{
    public class ProfileDto : IMapTo<Profile>
    {
        [Range(12, 120)]
        public int? Age { get; set; }
        [RegularExpression(@"^M|F|m|f$")]
        public string Sex { get; set; }
        [MaxLength(20)]
        public string Location { get; set; }
    }
}
