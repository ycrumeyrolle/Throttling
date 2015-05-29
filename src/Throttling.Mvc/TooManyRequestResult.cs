using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;

namespace Throttling.Mvc
{
    public class TooManyRequestResult : ActionResult
    {
        private readonly IReadableStringCollection _headers;
        private readonly string _retryAfter;

        public TooManyRequestResult(IReadableStringCollection headers, string retryAfter)
        {
            _headers = headers;
            _retryAfter = retryAfter;
        }

        public override void ExecuteResult(ActionContext context)
        {
            var response = context.HttpContext.Response;
  
            context.HttpContext.Response.StatusCode = 429;

            // rfc6585 section 4 : Responses with the 429 status code MUST NOT be stored by a cache.
            context.HttpContext.Response.Headers.SetValues("Cache-Control", "no-store", "no-cache");
            context.HttpContext.Response.Headers.Set("Pragma", "no-cache");

            // rfc6585 section 4 : The response [...] MAY include a Retry-After header indicating how long to wait before making a new request.
            if (_retryAfter != null)
            {
                response.Headers.Set("Retry-After", _retryAfter);
            }

            base.ExecuteResult(context);
        }
    }
}
