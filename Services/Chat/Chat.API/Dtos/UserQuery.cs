using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Dtos
{
    public class UserQuery
    {
        public string Id { get; }
        public string UserName { get; }
        public string Email { get; }

        public UserQuery(string id, string userName, string email)
        {
            Id = id;
            UserName = userName;
            Email = email;
        }
    }
}
