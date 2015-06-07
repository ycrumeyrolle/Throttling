using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottleRouter
    {
        void Add([NotNull] ThrottleRoute route);

        ThrottleStrategy GetThrottleStrategyAsync([NotNull] HttpContext httpContext, [NotNull] ThrottleOptions options);

        int Count { get; }

        IDictionary<string, ThrottlePolicy> PolicyMap { get; }
    }
}