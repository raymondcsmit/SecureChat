using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chat.Domain.AggregateModel.UserAggregate;
using Helpers.Mapping;

namespace Chat.API.Dtos
{
    public class UserUpdateDto: IMapTo<Profile>, IMapTo<User>
    {
        [MinLength(3), MaxLength(20)]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Range(12, 120)]
        public string Age { get; set; }
        [RegularExpression(@"^M|F|m|f$")]
        public string Sex { get; set; }
        [MinLength(5), MaxLength(20)]
        public string Location { get; set; }
    }
}
