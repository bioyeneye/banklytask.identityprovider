using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace IdentityProvider.Filters
{
    public class ValidateModelStateActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Check if ModelState is valid.
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToList();
                context.Result = new BadRequestObjectResult(new ApiError(StatusCodes.Status400BadRequest, "Model validation error", "Model input is not correct", errors));
            }
            else
            {
                // ModelState is valid. Go to next filter.
                await next();
            }
        }
    }
    
    public class ApiError
    {
        public int StatusCode { get; private set; }
        public string StatusDescription { get; private set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; private set; }
        public List<string> Errors { get; private set; }

        public ApiError(int statusCode, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.StatusDescription = statusDescription;
        }

        public ApiError(int statusCode, string statusDescription, string message, List<string> errors)
            : this(statusCode, statusDescription)
        {
            this.Message = message;
            this.Errors = errors;
        }
    }
}