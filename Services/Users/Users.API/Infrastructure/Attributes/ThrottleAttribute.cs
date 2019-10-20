using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Users.API.Models;

namespace Users.API.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ThrottleAttribute : ActionFilterAttribute
    {
        public int MilliSeconds { get; set; }
        public string Message { get; set; }

        private static MemoryCache Cache { get; set; } = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 3000 });

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development")
            {
                return;
            }

            var actionName = context.ActionDescriptor.DisplayName;
            var key = string.Concat(actionName, "-", context.HttpContext.Request.HttpContext.Connection.RemoteIpAddress);

            if (!Cache.TryGetValue(key, out bool entry))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMilliseconds(MilliSeconds))
                    .SetSize(1);

                Cache.Set(key, true, cacheEntryOptions);
            }
            else
            {
                if (string.IsNullOrEmpty(Message))
                    Message = $"You may only perform this action every {(double)MilliSeconds / 1000} seconds.";

                context.Result = new ObjectResult(new ErrorResponse(new []{Message}));
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        // To reset the cache after unit tests
        public static void ResetCache()
        {
            Cache.Dispose();
            Cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = 1000 });
        }
    }
}