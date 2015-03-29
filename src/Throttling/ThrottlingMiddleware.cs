using System;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.Logging;

namespace Throttling
{
    /// <summary>
    /// An ASP.NET middleware for handling Throttling.
    /// </summary>
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IThrottlingService _throttlingService;
        private readonly IThrottlingPolicyProvider _throttlingPolicyProvider;
        private readonly ThrottlingPolicy _policy;
        private readonly ILogger _logger;
        private readonly string _policyName;

        /// <summary>
        /// Instantiates a new <see cref="ThrottlingMiddleware"/>.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="ThrottlingService">An instance of <see cref="IThrottlingService"/>.</param>
        /// <param name="policyProvider">A policy provider which can get an <see cref="ThrottlingPolicy"/>.</param>
        /// <param name="policyName">An optional name of the policy to be fetched.</param>
     	public ThrottlingMiddleware([NotNull] RequestDelegate next, [NotNull] ILoggerFactory loggerFactory, [NotNull] IThrottlingService throttlingService, [NotNull] IThrottlingPolicyProvider policyProvider, [NotNull] string policyName)
        {
            _next = next;
            _logger = LoggerFactoryExtensions.CreateLogger<ThrottlingMiddleware>(loggerFactory);
            _throttlingService = throttlingService;
            _throttlingPolicyProvider = policyProvider;
            _policyName = policyName;
        }
        /// <summary>
        /// Instantiates a new <see cref="T:Throttling.ThrottlingMiddleware" />.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="throttlingService">An instance of <see cref="T:Throttling.IThrottlingService" />.</param>
        /// <param name="policy">An instance of the <see cref="T:Throttling.ThrottlingPolicy" /> which can be applied.</param>
        public ThrottlingMiddleware([NotNull] RequestDelegate next, [NotNull] ILoggerFactory loggerFactory, [NotNull] IThrottlingService throttlingService, [NotNull] ThrottlingPolicy policy)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ThrottlingMiddleware>();
            _throttlingService = throttlingService;
            _policy = policy;
        }

        /// <inheritdoc />
        public async Task Invoke(HttpContext context)
        {
            var throttlingPolicy = _policy ?? await _throttlingPolicyProvider?.GetThrottlingPolicyAsync(context, _policyName);
            if (throttlingPolicy != null)
            {
                var throttlingResults = await _throttlingService.EvaluatePolicyAsync(context, throttlingPolicy);
                if (!_throttlingService.ApplyResult(context, throttlingResults))
                {
                    await _next(context);
                }
            }
        }
    }
}
