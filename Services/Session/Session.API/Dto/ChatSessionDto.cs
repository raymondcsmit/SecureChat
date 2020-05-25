using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.Dto
{
    public class ChatSessionDto
    {
        public DateTimeOffset StartTime { get; set; }

        public int IdleSeconds { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}
