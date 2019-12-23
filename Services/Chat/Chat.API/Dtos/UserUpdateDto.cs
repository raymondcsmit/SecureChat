using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Dtos
{
    public class UserUpdateDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Age { get; set; }

        public string Sex { get; set; }

        public string Location { get; set; }
    }
}
