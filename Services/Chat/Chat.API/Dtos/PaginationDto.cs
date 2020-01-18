using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.API.Dtos
{
    public class PaginationDto
    {
        public int Limit { get; }
        public int Offset { get; }

        public PaginationDto(int limit, int offset)
        {
            Limit = limit;
            Offset = offset;
        }
    }
}
