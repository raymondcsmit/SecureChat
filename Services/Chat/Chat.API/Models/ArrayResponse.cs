using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Models
{
    public class ArrayResponse<T>
    {
        public int Total => Items.Count;

        public IReadOnlyList<T> Items { get; }

        public ArrayResponse(IReadOnlyList<T> items)
        {
            Items = items;
        }
    }
}
