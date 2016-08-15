using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class IPRequirement : ThrottleRequirement, IKeyProvider
    {
        protected IPRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding)
            : base(maxValue, renewalPeriod, sliding)
        {
        }

        public string GetKey(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            IHttpConnectionFeature connection = httpContext.Features.Get<IHttpConnectionFeature>();
            return connection.RemoteIpAddress.ToString();
        }
    }
}