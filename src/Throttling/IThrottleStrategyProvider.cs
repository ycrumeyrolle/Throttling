using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;

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
        Task<ThrottleStrategy> GetThrottleStrategyAsync(HttpContext httpContext, string policyName, IThrottleRouter router);
    }
}