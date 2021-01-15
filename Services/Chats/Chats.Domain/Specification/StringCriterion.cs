using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chats.Domain.Specification
{
    public class StringCriterion
    {
        public string Property { get; }
        public string Value { get; }
        public string Mode { get; }

        public StringCriterion(string property, string value, string mode)
        {
            Property = property ?? throw new ArgumentException();
            Value = value ?? throw new ArgumentException();

            if (!new[] { ComparisonMode.Equal, ComparisonMode.GreaterThan, ComparisonMode.LessThan }.Contains(mode))
            {
                throw new ArgumentException();
            }
            Mode = mode;
        }
    }
}
