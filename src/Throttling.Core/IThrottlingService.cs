using System.Collections.Generic;
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
        Task<IEnumerable<ThrottlingResult>> EvaluatePolicyAsync(HttpContext context, IThrottlingPolicy policy);
       
        /// <summary>
        /// Apply result to the given <paramref name="response" />.
        /// </summary>
        /// <param name="response">The <see cref="T:Microsoft.AspNet.Http.HttpResponse" /> associated with the current call.</param>
        /// <param name="result">The <see cref="T:Throttling.ThrottlingResult" /> used to read the allowed values.</param>
        /// <returns><c>true</c> if the result implies a "Too many requests" response; <c>false otherwise</c>.</returns>
        bool ApplyResult([NotNull] HttpContext context, [NotNull] IEnumerable<ThrottlingResult> result);
    }
}