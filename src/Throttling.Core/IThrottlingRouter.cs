using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottlingRouter
    {
        void Add(ThrottlingRoute route);

        ThrottlingStrategy GetThrottlingStrategyAsync([NotNull] HttpContext context, [NotNull] ThrottlingOptions options);

        int Count { get; }

        IDictionary<string, IThrottlingPolicy> PolicyMap { get; }
    }
}