using System;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class AuthenticatedUserRequirement : ThrottlingRequirement, IKeyProvider
    {
        protected AuthenticatedUserRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
        }

        public string GetKey([NotNull] HttpContext httpContext)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return httpContext.User.Identity.Name;
            }

            return null;
        }
    }
}