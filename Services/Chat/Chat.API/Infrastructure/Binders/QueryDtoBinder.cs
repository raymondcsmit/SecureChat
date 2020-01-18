using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Chat.API.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Chat.API.Infrastructure.Binders
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
            catch (JsonException)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
