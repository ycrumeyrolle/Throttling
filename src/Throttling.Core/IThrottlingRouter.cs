using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottlingRouter
    {
        void Add([NotNull] ThrottlingRoute route);

        ThrottlingStrategy GetThrottlingStrategyAsync([NotNull] HttpContext httpContext, [NotNull] ThrottlingOptions options);

        int Count { get; }

        IDictionary<string, ThrottlingPolicy> PolicyMap { get; }
    }
}