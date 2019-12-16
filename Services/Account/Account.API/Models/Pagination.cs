using Account.API.Infrastructure.Binders;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Models
{
    [ModelBinder(BinderType = typeof(PaginationBinder))]
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
