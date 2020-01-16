using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Models
{
    public class ArrayResponse<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
