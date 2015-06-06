using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class IPRequirement : ThrottlingRequirement, IKeyProvider
    {
        protected IPRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
        }

        public string GetKey([NotNull]HttpContext context)
        {
            // TODO : What if behind reverse proxy? X-Forwarded-For ?
            IHttpConnectionFeature connection = context.GetFeature<IHttpConnectionFeature>();
            return connection.RemoteIpAddress.ToString();
        }
    }
}