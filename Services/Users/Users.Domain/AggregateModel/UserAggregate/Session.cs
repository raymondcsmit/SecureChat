using System;
using Users.Domain.SeedWork;

namespace Users.Domain.AggregateModel.UserAggregate
{
    public class Session : Entity
    {
        private const int IdleTime = 30;

        public DateTimeOffset StartTime { get; private set; }

        public DateTimeOffset LastActivityTime { get; private set; }

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
