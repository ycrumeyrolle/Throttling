using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottlingService
    {
        /// <summary>
        /// Evaluates the given <paramref name="policy" /> using the passed in <paramref name="context" />.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNet.Http.HttpContext" /> associated with the call.</param>
        /// <param name="policy">The <see cref="T:Throttling.IThrottlingPolicy" /> which needs to be evaluated.</param>
        /// <returns>A <see cref="T:Throttling.ThrottlingResult" /> which contains the result of policy evaluation.</returns>
        Task<ThrottlingContext> EvaluateAsync([NotNull] HttpContext context, [NotNull] ThrottlingStrategy strategy);

        Task PostEvaluateAsync([NotNull] ThrottlingContext throttlingContext);
    }
}