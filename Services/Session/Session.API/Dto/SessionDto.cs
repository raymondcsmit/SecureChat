using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.Dto
{
    public class SessionDto
    {
        public DateTimeOffset StartTime { get; set; }

        public TimeSpan IdleTime { get; set; }

        public DateTimeOffset PreviousEndTime { get; set; }
    }
}
