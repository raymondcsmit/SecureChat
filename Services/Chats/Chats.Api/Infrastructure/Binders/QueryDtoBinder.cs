using System;
using System.Threading.Tasks;
using Chats.Api.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Chats.Api.Infrastructure.Binders
{
    public class QueryDtoBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var query = bindingContext.ValueProvider.GetValue("query").FirstValue;
            try
            {
                var queryDto = JsonConvert.DeserializeObject<QueryDto>(query);
                bindingContext.Result = ModelBindingResult.Success(queryDto);
            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Success(new QueryDto());
            }

            return Task.CompletedTask;
        }
    }
}
