using Session.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Session.API.Models;

namespace Session.API.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is SessionApiException e)
            {
                if (e.ErrorCode == 404)
                {
                    context.Result = new NotFoundResult();
                }
                else
                {
                    context.Result = new BadRequestObjectResult(new ErrorResponse(e.Errors));
                }
                
            }
            else
            {
                _logger.LogError(new EventId(context.Exception.HResult),
                    context.Exception,
                    context.Exception.Message);

                var message = context.Exception.Message;

                context.Result = new ObjectResult(new ErrorResponse(new[] { message }))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            context.ExceptionHandled = true;
        }
    }
}
