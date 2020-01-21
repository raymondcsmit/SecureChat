using System;
using System.Collections.Generic;
using System.Text;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;

namespace Chat.Domain.AggregateModel.UserAggregate
{
    public class Profile: Entity
    {
        public int Age { get; set; }

        public string Sex { get; set; }

        public string Location { get; set; }

        private Profile() { }

        public Profile(int age, string sex, string location)
        {
            if (age == default || sex == default || location == default)
            {
                throw new ChatDomainException("All fields of profile are required");
            }
            Age = age;
            Sex = sex;
            Location = location;
        }
    }
}
