using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Throttling;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which applies the given <see cref="ThrottlingPolicy"/> and adds appropriate response headers.
    /// </summary>
    public class ThrottlingAuthorizationFilter : IThrottlingAuthorizationFilter
    {
        private readonly IThrottlingService _throttlingService;
        private readonly IThrottlingPolicyProvider _throttlingPolicyProvider;
        private readonly ThrottlingOptions _options;

        /// <summary>
        /// Creates a new instace of <see cref="ThrottlingAuthorizationFilter"/>.
        /// </summary>
        /// <param name="ThrottlingService">The <see cref="IThrottlingService"/>.</param>
        /// <param name="policyProvider">The <see cref="IThrottlingPolicyProvider"/>.</param>
        public ThrottlingAuthorizationFilter(IOptions<ThrottlingOptions> optionsAccessor, IThrottlingService ThrottlingService, IThrottlingPolicyProvider policyProvider)
        {
            _throttlingService = ThrottlingService;
            _throttlingPolicyProvider = policyProvider;
            _options = optionsAccessor.Options;
        }

        /// <inheritdoc />
        public int Order
        {
            get
            {
                return -1;
            }
        }

        public string PolicyName { get; set; }

        public ThrottlingRoute Route { get; set; }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync([NotNull] AuthorizationContext context)
        {
            //// If this filter is not closest to the action, it is not applicable.
            //if (!IsClosestToAction(context.Filters))
            //{
            //    return;
            //}

            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            var strategy = await _throttlingPolicyProvider?.GetThrottlingStrategyAsync(httpContext, PolicyName);
            if (strategy == null && Route.Match(request))
            {
                strategy = new ThrottlingStrategy
                {
                    Policy = Route.GetPolicy(httpContext.Request, _options),
                    RouteTemplate = Route.RouteTemplate
                };
            }

            if (strategy != null)
            {
                var throttlingResults = await _throttlingService.EvaluateStrategyAsync(httpContext, strategy);
                if (_throttlingService.ApplyResult(httpContext, throttlingResults))
                {
                    context.Result = new HttpStatusCodeResult(429);
                }
                else
                {
                    await _throttlingService.ApplyLimitAsync(throttlingResults);
                }
            }
        }

        private bool IsClosestToAction(IEnumerable<IFilter> filters)
        {
            // If there are multiple IThrottlingAuthorizationFilter which are defined at the class and
            // at the action level, the one closest to the action overrides the others. 
            // Since filterdescriptor collection is ordered (the last filter is the one closest to the action),
            // we apply this constraint only if there is no IThrottlingAuthorizationFilter after this.
            return filters.Last(filter => filter is IThrottlingAuthorizationFilter) == this;
        }
    }
}
