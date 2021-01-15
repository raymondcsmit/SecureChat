using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chats.Domain.Specification
{
    public class StringOrderBy
    {
        public string Key { get; }
        public string Mode { get; }

        public StringOrderBy(string property, string mode)
        {
            if (!new[] { OrderByMode.Ascending, OrderByMode.Descending }.Contains(mode))
            {
                throw new ArgumentException();
            }

            Key = property;
            Mode = mode;
        }
    }
}
