using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chats.Api.Dtos
{
    public class ErrorResponse
    {
        public IEnumerable<string> Errors { get; set; }

        public ErrorResponse(ModelStateDictionary modelState)
        {
            Errors = modelState
                .SelectMany(kvp => kvp.Value.Errors)
                .Select(error => error.ErrorMessage);
        }

        public ErrorResponse(IEnumerable<string> errors = null)
        {
            Errors = errors;
        }
    }
}
