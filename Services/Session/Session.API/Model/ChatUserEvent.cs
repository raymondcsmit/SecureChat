using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.Model
{
    public class ChatUserEvent
    {
        public ChatUser User { get; set; }
        public string UserId { get; set; }

        public ChatEvent Event { get; set; }
        public int EventId { get; set; }
    }
}
