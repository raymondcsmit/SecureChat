using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chats.Api.Dtos;
using Chats.Domain.Specification;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Chats.Api.Infrastructure.Binders
{
    public class QueryDtoBinder : IModelBinder
    {
        private const string _defaultSortMode = "ASC";

        private IEnumerable<string> _validCriteriaModes = new List<string>() { "=", "<", ">" };

        private IEnumerable<string> _validSortModes = new List<string>() { "ASC", "DESC" };

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var criteria = bindingContext.ValueProvider.GetValue("criteria").FirstValue;
            var orderBy = bindingContext.ValueProvider.GetValue("sort").FirstValue;
            var pagination = bindingContext.ValueProvider.GetValue("pagination").FirstValue;

            var criteriaCollection = new List<StringCriterion>();
            if (criteria != null)
            {
                var criteriaModes = string.Join('|', _validCriteriaModes);
                if (!Regex.Matches(criteria, GetCriteriaPattern(criteriaModes)).Any())
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    bindingContext.ModelState.TryAddModelError(
                        bindingContext.ModelName,
                        "Invalid criteria query string format");
                }
                criteriaCollection = GetStringCriteria(criteria);
            }

            var orderByCollection = new List<StringOrderBy>();
            if (orderBy != null)
            {
                var sortModes = string.Join('|', _validSortModes);
                if (!Regex.Matches(orderBy, GetSortPattern(sortModes)).Any())
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    bindingContext.ModelState.TryAddModelError(
                        bindingContext.ModelName,
                        "Invalid sort query string format");
                }
                orderByCollection = GetStringOrderBy(orderBy);
            }

            PaginationDto paginationDto = null;
            if (pagination != null)
            {
                try
                {
                    paginationDto = JsonConvert.DeserializeObject<PaginationDto>(pagination);
                }
                catch (Exception)
                {
                    bindingContext.ModelState.TryAddModelError(
                        bindingContext.ModelName,
                        "Invalid pagination query string format");
                }
            }

            if (bindingContext.ModelState.ErrorCount > 0)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            var queryDto = new QueryDto()
            {
                Criteria = criteriaCollection,
                OrderBy = orderByCollection,
                Pagination = paginationDto
            };
            bindingContext.Result = ModelBindingResult.Success(queryDto);

            await Task.CompletedTask;
        }

        private List<StringCriterion> GetStringCriteria(string queryValue)
            => queryValue
                ?.Split(',')
                ?.Select(str =>
                {
                    var criterion = str
                        .Split('|')
                        .Select(s => s.Trim())
                        .ToArray();
                    return new StringCriterion(
                        property: criterion[0],
                        value: criterion[1],
                        mode: criterion[2]);
                })
                ?.ToList() ?? new List<StringCriterion>();

        private List<StringOrderBy> GetStringOrderBy(string queryValue)
            => queryValue
                ?.Split(',')
                ?.Select(str =>
                {
                    var sort = str
                        .Split('|')
                        .Select(s => s.Trim())
                        .ToArray();
                    return new StringOrderBy(
                        property: sort[0],
                        mode: sort.ElementAtOrDefault(1) ?? _defaultSortMode);
                })
                ?.ToList() ?? new List<StringOrderBy>();

        private string GetCriteriaPattern(string mode)
            => @$"^(\s*[a-zA-Z_][a-zA-Z_0-9]*\s*\|\s*\w*\s*\|\s*({mode})\s*)(,\s*[a-zA-Z_][a-zA-Z_0-9]*\s*\|\s*\w*\s*\|\s*({mode})\s*)*$";
        
        private string GetSortPattern(string mode)
            => @$"^(\s*[a-zA-Z_][a-zA-Z_0-9]*\s*(\|\s*({mode})\s*)?)(,\s*[a-zA-Z_][a-zA-Z_0-9]*\s*(\|\s*({mode})\s*)?)*$";


    }
}
