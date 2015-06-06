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

        public string GetKey([NotNull] HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name;
            }

            return null;
        }
    }
}