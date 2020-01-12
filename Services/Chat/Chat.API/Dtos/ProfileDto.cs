using System.ComponentModel.DataAnnotations;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;

namespace Chat.API.Dtos
{
    public class ProfileDto : IMapTo<Profile>
    {
        [Required, Range(12, 120)]
        public int? Age { get; set; }
        [RegularExpression(@"^M|F|m|f$")]
        public string Sex { get; set; }
        [MaxLength(20)]
        public string Location { get; set; }
    }
}
