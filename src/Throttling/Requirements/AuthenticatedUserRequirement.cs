using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class AuthenticatedUserRequirement : ThrottleRequirement, IKeyProvider
    {
        protected AuthenticatedUserRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
        }

        public string GetKey(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (httpContext.User.Identity.IsAuthenticated)
            {
                return httpContext.User.Identity.Name;
            }

            return null;
        }
    }
}