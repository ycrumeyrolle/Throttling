using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottleStrategyProvider
    {
        /// <summary>
        /// Selects a throttle policy to apply for the given request.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> associated with this call.</param>
        /// <param name="policyName">An optional policy name to look for.</param>
        /// <returns>A <see cref="IThrottlePolicy"/></returns>
        Task<ThrottleStrategy> GetThrottleStrategyAsync([NotNull] HttpContext httpContext, string policyName);
    }
}