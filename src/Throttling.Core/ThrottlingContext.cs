using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottlingContext
    {
        private HashSet<IThrottlingRequirement> _pendingRequirements;
        private bool _tooManyRequestCalled;
        private bool _succeedCalled;

        public ThrottlingContext([NotNull] HttpContext httpContext, [NotNull] ThrottlingStrategy strategy)
        {
            Headers = new HeaderDictionary();
            HttpContext = httpContext;
            Requirements = strategy.Policy.Requirements;
            RouteTemplate = strategy.RouteTemplate;
            _pendingRequirements = new HashSet<IThrottlingRequirement>(Requirements);
        }

        public HttpContext HttpContext { get; set; }

        public IEnumerable<IThrottlingRequirement> Requirements { get; private set; }

        public string RouteTemplate { get; private set; }

        public IEnumerable<IThrottlingRequirement> PendingRequirements { get { return _pendingRequirements; } }

        public bool HasFailed { get { return _tooManyRequestCalled; } }

        public bool HasSucceeded
        {
            get
            {
                return !_tooManyRequestCalled && _succeedCalled && !PendingRequirements.Any();
            }
        }

        public DateTimeOffset? RetryAfter { get; set; }

        public IHeaderDictionary Headers { get; private set; }

        public void Succeed([NotNull] IThrottlingRequirement requirement)
        {
            _succeedCalled = true;
            _pendingRequirements.Remove(requirement);
        }

        public void TooManyRequest<TRequirement>([NotNull] TRequirement requirement, DateTimeOffset retryAfter) where TRequirement : LimitRateRequirement
        {
            _tooManyRequestCalled = true;
            if (!RetryAfter.HasValue || RetryAfter.Value < retryAfter)
            {
                RetryAfter = retryAfter;
            }
        }
    }
}