using System.Net;

namespace IdentityProvider.BusinessDomain.ServiceModels
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(bool isSuccessful, string message, HttpStatusCode statusCode)
        {
            IsSuccessful = isSuccessful;
            Message = message;
            StatusCode = statusCode;
        }

        public bool IsSuccessful { get; private set; }
        public string Message { get; private set; }
        public HttpStatusCode StatusCode { get; private set; }
    }
}