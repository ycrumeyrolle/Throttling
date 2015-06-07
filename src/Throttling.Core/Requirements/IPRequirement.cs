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

        public string GetKey([NotNull]HttpContext httpContext)
        {
            // TODO : What if behind reverse proxy? X-Forwarded-For ?
            IHttpConnectionFeature connection = httpContext.GetFeature<IHttpConnectionFeature>();
            return connection.RemoteIpAddress.ToString();
        }
    }
}