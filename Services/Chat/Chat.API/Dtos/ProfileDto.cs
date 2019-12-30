using System.ComponentModel.DataAnnotations;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;

namespace Chat.API.Dtos
{
    public class ProfileDto : IMapTo<Profile>
    {
        [Range(12, 120)]
        public string Age { get; set; }
        [RegularExpression(@"^M|F|m|f$")]
        public string Sex { get; set; }
        [MinLength(5), MaxLength(20)]
        public string Location { get; set; }
    }
}
