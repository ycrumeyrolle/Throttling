using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Internal;

namespace Throttling
{
    public class ThrottleContext
    {
        private HashSet<IThrottleRequirement> _pendingRequirements;
        private bool _tooManyRequestCalled;
        private bool _succeedCalled;
        private bool _abortCalled;

        public ThrottleContext(HttpContext httpContext, ThrottleStrategy strategy)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

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

        public void Succeed(IThrottleRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            _succeedCalled = true;
            _pendingRequirements.Remove(requirement);
        }

        public void Skipped(IThrottleRequirement requirement)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            _pendingRequirements.Remove(requirement);
        }

        public void TooManyRequest(IThrottleRequirement requirement, DateTimeOffset retryAfter)
        {
            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            _tooManyRequestCalled = true;
            if (!RetryAfter.HasValue || RetryAfter.Value < retryAfter)
            {
                RetryAfter = retryAfter;
            }
        }

        public void Abort(IThrottleExclusion exclusion)
        {
            if (exclusion == null)
            {
                throw new ArgumentNullException(nameof(exclusion));
            }

            _abortCalled = true;
        }
    }
}