using System;
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
                var errors = context.ModelState.Keys.Where(i => context.ModelState[i].Errors.Count > 0)
                    .Select(k => new ValidationErrorModel
                    {
                        Field = k,
                        Message = context.ModelState[k].Errors.First().ErrorMessage
                    }).ToList();

                context.Result = new BadRequestObjectResult(new ApiError<ValidationErrorModel>(StatusCodes.Status400BadRequest, "Your request parameters didn't validate", "Model input is not correct", errors));
            }
            else
            {
                // ModelState is valid. Go to next filter.
                await next();
            }
        }
    }

    public class ApiError<T>
    {
        public ApiError(int statusCode, string statusDescription)
        {
            Id = Guid.NewGuid().ToString();
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            ErrorTime = DateTime.UtcNow;
        }

        public ApiError(int statusCode, string statusDescription, string message, List<T> errors)
            : this(statusCode, statusDescription)
        {
            Message = message;
            Errors = errors;
        }

        public string Id { get; set; }
        public int StatusCode { get; }
        public string StatusDescription { get; }
        public DateTime ErrorTime { get; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Message { get; }
        public List<T> Errors { get; }
    }

    public class ValidationErrorModel
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
}