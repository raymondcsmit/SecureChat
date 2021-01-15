using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chats.Domain.Specification
{
    public class StringInclude
    {
        public string Include { get; }
        public string ThenInclude { get; }

        public StringInclude(string include, string thenInclude)
        {
            Include = include ?? throw new ArgumentException();
            ThenInclude = thenInclude;
        }
    }
}
