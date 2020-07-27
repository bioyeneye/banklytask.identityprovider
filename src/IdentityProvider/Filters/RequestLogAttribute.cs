using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace IdentityProvider.Filters
{
    public class RequestLogAttribute : TypeFilterAttribute
    {
        public RequestLogAttribute() : base(typeof(RequestLogActionFilter))
        {
        }
        
        private class RequestLogActionFilter : IActionFilter
        {
            private readonly ILogger _logger;
            public RequestLogActionFilter(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<RequestLogAttribute>();
            }

            public async void OnActionExecuting(ActionExecutingContext context)
            {
                // perform some business logic work
                var buffer = new byte[Convert.ToInt32(context.HttpContext.Request.ContentLength)];
                await context.HttpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                var requestBody = Encoding.UTF8.GetString(buffer);
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                var builder = new StringBuilder(Environment.NewLine);
                foreach (var header in context.HttpContext.Request.Headers)
                {
                    builder.AppendLine($"{header.Key}:{header.Value}");
                }

                builder.AppendLine($"Request body:{requestBody}");

                _logger.LogInformation($"request: {builder.ToString()}");
               // _logger.LogInformation($"path: {context.HttpContext.Request.Path}"); 
                
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                //TODO: log body content and response as well
                _logger.LogInformation($"path: {context.HttpContext.Request.Path}"); 
                
                
            }
        }
    }
    
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var builder = new StringBuilder(Environment.NewLine);
            foreach (var header in context.Request.Headers)
            {
                builder.AppendLine($"{header.Key}:{header.Value}");
            }

            builder.AppendLine($"Request body:{requestBody}");

            logger.LogInformation(builder.ToString());

            await next(context);
        }
    }
}