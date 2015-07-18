using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottleContext
    {
        private HashSet<IThrottleRequirement> _pendingRequirements;
        private bool _tooManyRequestCalled;
        private bool _succeedCalled;
        private bool _abortCalled;

        public ThrottleContext([NotNull] HttpContext httpContext, [NotNull] ThrottleStrategy strategy)
        {
            HttpContext = httpContext;
            Requirements = strategy.Policy.Requirements;
            Exclusions = strategy.Policy.Exclusions;
            RouteTemplate = strategy.RouteTemplate;
            _pendingRequirements = new HashSet<IThrottleRequirement>(Requirements);
        }
        
        public HttpContext HttpContext { get; }

        public IEnumerable<IThrottleRequirement> Requirements { get; }

        public IEnumerable<IThrottleExclusion> Exclusions { get; }

        public string RouteTemplate { get; }

        public DateTimeOffset? RetryAfter { get; set; }

        public IHeaderDictionary ResponseHeaders { get; } = new HeaderDictionary();

        public ContentLengthTracker ContentLengthTracker { get; set; }

        public IEnumerable<IThrottleRequirement> PendingRequirements
        {
            get { return _pendingRequirements; }
        }

        public bool HasTooManyRequest
        {
            get { return _tooManyRequestCalled; }
        }

        public bool HasSucceeded
        {
            get { return !_tooManyRequestCalled && _succeedCalled && !PendingRequirements.Any(); }
        }

        public bool HasAborted
        {
            get
            {
                return _abortCalled;
            }
        }

        public void Succeed([NotNull] IThrottleRequirement requirement)
        {
            _succeedCalled = true;
            _pendingRequirements.Remove(requirement);
        }

        public void Skipped([NotNull] IThrottleRequirement requirement)
        {
            _pendingRequirements.Remove(requirement);
        }

        public void TooManyRequest([NotNull] IThrottleRequirement requirement, DateTimeOffset retryAfter)
        {
            _tooManyRequestCalled = true;
            if (!RetryAfter.HasValue || RetryAfter.Value < retryAfter)
            {
                RetryAfter = retryAfter;
            }
        }

        public void Abort([NotNull] IThrottleExclusion exclusion)
        {
            _abortCalled = true;
        }
    }
}