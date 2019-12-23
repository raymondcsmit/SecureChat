using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class Profile: Entity
    {
        public string Age { get; set; }

        public string Sex { get; set; }

        public string Location { get; set; }

        public Profile(string age, string sex, string location)
        {
            Age = age;
            Sex = sex;
            Location = location;
        }
    }
}
