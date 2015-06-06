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
        private bool _abortCalled;

        public ThrottlingContext([NotNull] HttpContext httpContext, [NotNull] ThrottlingStrategy strategy)
        {
            HttpContext = httpContext;
            Requirements = strategy.Policy.Requirements;
            Exclusions = strategy.Policy.Exclusions;
            RouteTemplate = strategy.RouteTemplate;
            _pendingRequirements = new HashSet<IThrottlingRequirement>(Requirements);
        }


        public HttpContext HttpContext { get; set; }

        public IEnumerable<IThrottlingRequirement> Requirements { get; }

        public IEnumerable<IThrottlingExclusion> Exclusions { get; }

        public string RouteTemplate { get; }

        public DateTimeOffset? RetryAfter { get; set; }

        public IHeaderDictionary Headers { get; } = new HeaderDictionary();

        public ContentLengthTracker ContentLengthTracker { get; } = new ContentLengthTracker();

        public IEnumerable<IThrottlingRequirement> PendingRequirements
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

        public void Succeed([NotNull] IThrottlingRequirement requirement)
        {
            _succeedCalled = true;
            _pendingRequirements.Remove(requirement);
        }

        public void TooManyRequest([NotNull] IThrottlingRequirement requirement, DateTimeOffset retryAfter)
        {
            _tooManyRequestCalled = true;
            if (!RetryAfter.HasValue || RetryAfter.Value < retryAfter)
            {
                RetryAfter = retryAfter;
            }
        }

        public void Abort([NotNull] IThrottlingExclusion exclusion)
        {
            _abortCalled = true;
        }
    }
}