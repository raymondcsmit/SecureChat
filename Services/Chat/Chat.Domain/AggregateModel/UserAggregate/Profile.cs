using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class Profile: Entity
    {
        public string Age { get; private set; }

        public string Sex { get; private set; }

        public string Location { get; private set; }

        private Profile() { }

        public Profile(string age, string sex, string location)
        {
            Age = age;
            Sex = sex;
            Location = location;
        }
    }
}
