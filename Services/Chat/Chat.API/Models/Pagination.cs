using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Models
{
    public class Pagination
    {
        public static Pagination Default => new Pagination()
        {
            Limit = int.MaxValue,
            Offset = 0
        };

        public int Limit { get; set; }
        public int Offset { get; set; }
    }
}
