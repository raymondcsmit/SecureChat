using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;

namespace Registration.Exceptions
{
    public class ApiException : Exception
    {
        private class ApiError
        {
            public IEnumerable<string> Errors { get; set; }
        }

        public IEnumerable<string> Errors { get; }

        public ApiException(IEnumerable<string> errors)
        {
            Errors = errors;
        }

        public ApiException()
        {
            Errors = Enumerable.Empty<string>();
        }

        public static async Task<ApiException> FromHttpResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException();
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<ApiError>(content);
            try
            {
                return new ApiException(json.Errors);
            }
            catch (RuntimeBinderException)
            {
                return new ApiException(new[] {"An error occured while communicating with the API"});
            }
        }
    }
}
