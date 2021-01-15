using Chats.Api.Dtos;
using Chats.Api.Infrastructure.Exceptions;
using Chats.Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Chats.Api.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(IWebHostEnvironment env, ILogger<GlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);
            switch (context.Exception)
            {
                case ChatApiException e:
                    if (e.ErrorCode == 404)
                    {
                        context.Result = new NotFoundResult();
                    }
                    else
                    {
                        context.Result = new BadRequestObjectResult(new ErrorResponse(e.Errors));
                    }
                    break;
                case ChatDomainException e:
                    context.Result = new BadRequestObjectResult(new ErrorResponse(e.Errors));
                    break;
                default:
                    var message = _env.IsDevelopment() ? context.Exception.Message : "An error has occured.";

                    context.Result = new ObjectResult(new ErrorResponse(new[] { message }))
                    {
                        StatusCode = StatusCodes.Status500InternalServerError
                    };
                    break;
            }

        }
    }
}
