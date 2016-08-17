using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Throttling.Mvc
{
    public class TooManyRequestResult : ActionResult
    {
        private readonly string _retryAfter;

        public TooManyRequestResult(string retryAfter)
        {
            if (retryAfter == null)
            {
                throw new ArgumentNullException(nameof(retryAfter));
            }

            _retryAfter = retryAfter;
        }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.HttpContext.Response;

            response.StatusCode = Constants.Status429TooManyRequests;

            // rfc6585 section 4 : Responses with the 429 status code MUST NOT be stored by a cache.
            response.Headers.SetCommaSeparatedValues("Cache-Control", "no-store", "no-cache");
            response.Headers["Pragma"] = "no-cache";

            // rfc6585 section 4 : The response [...] MAY include a Retry-After header indicating how long to wait before making a new request.
            if (_retryAfter != null)
            {
                response.Headers["Retry-After"] = _retryAfter;
            }

            base.ExecuteResult(context);
        }
    }
}
