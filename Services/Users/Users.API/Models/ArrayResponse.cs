using System.Collections.Generic;

namespace Users.API.Models
{
    public class ArrayResponse<T>
    {
        public int Total { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
