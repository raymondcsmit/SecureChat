using System;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class Session : Entity
    {
        private const int IdleTime = 30;

        public DateTimeOffset StartTime { get; protected set; }

        public DateTimeOffset LastActivityTime { get; protected set; }

        protected Session() { }

        public static Session New => new Session()
        {
            StartTime = DateTimeOffset.Now,
            LastActivityTime = DateTimeOffset.Now
        };

        public bool IsIdle => DateTimeOffset.Now < StartTime.AddMinutes(IdleTime);

        public void Refresh()
        {
            LastActivityTime = DateTimeOffset.Now;
        }
    }
}
