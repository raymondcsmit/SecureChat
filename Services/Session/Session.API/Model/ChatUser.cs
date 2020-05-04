using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Session.API.Models;

namespace Session.API.Model
{
    public class ChatUser : IAuditable
    {
        public string Id { get; set; }

        public ICollection<ChatUserEvent> EventsLink { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
    }
}
