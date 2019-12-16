using System;
using System.Threading.Tasks;
using Account.API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Account.API.Infrastructure.Binders
{
    public class PaginationBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var limitValueResult = bindingContext.ValueProvider.GetValue(nameof(Pagination.Limit));
            var offsetValueResult = bindingContext.ValueProvider.GetValue(nameof(Pagination.Offset));

            try
            {
                var pagination = new Pagination()
                {
                    Limit = int.Parse(limitValueResult.FirstValue),
                    Offset = int.Parse(offsetValueResult.FirstValue)
                };
                bindingContext.Result = ModelBindingResult.Success(pagination);
            }
            catch
            {
                bindingContext.Result = ModelBindingResult.Success(Pagination.Default);
            }

            return Task.CompletedTask;
        }
    }
}
