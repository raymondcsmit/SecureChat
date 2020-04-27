using Session.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Session.API.Model
{
    public class ChatSession : IAuditable
    {
        private const int IdleTime = 30;

        public string Id { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        
        public DateTimeOffset ModifiedAt { get; set; }

        public bool IsIdle => DateTimeOffset.Now < ModifiedAt.AddMinutes(IdleTime);

        public void Refresh()
        {
            ModifiedAt = DateTimeOffset.Now;
        }
    }
}
