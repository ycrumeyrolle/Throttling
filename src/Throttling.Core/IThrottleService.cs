using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottleService
    {
        /// <summary>
        /// Evaluates the given <paramref name="policy" /> using the passed in <paramref name="context" />.
        /// </summary>
        /// <param name="httpContext">The <see cref="T:HttpContext" /> associated with the call.</param>
        /// <param name="policy">The <see cref="T:IThrottlePolicy" /> which needs to be evaluated.</param>
        /// <returns>A <see cref="T:ThrottleContext" /> which contains the result of policy evaluation.</returns>
        Task<ThrottleContext> EvaluateAsync([NotNull] HttpContext httpContext, [NotNull] ThrottleStrategy strategy);

        Task PostEvaluateAsync([NotNull] ThrottleContext throttleContext);
    }
}