using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ThrottlingAttribute : Attribute, IActionModelConvention
    {
        public ThrottlingAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        public string PolicyName { get; }

        public void Apply(ActionModel model)
        {

            model.Filters.Add(new ThrottlingAuthorizationFilterFactory(PolicyName));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IPThrottling : Attribute, IActionModelConvention
    {
        private readonly long _limit ;

        private readonly long _window;

        public IPThrottling(long limit, long window)
        {
            _limit = limit;
            _window = window;
        }

        public bool Sliding { get; set; }

        public void Apply(ActionModel model)
        {
            // TODO : Add a category based on Url template
            // TODO : Test with areas
            object policyObj;
            ThrottlingPolicy policy;
            if (!model.Properties.TryGetValue("Throttling.Policy", out policyObj))
            {
                policy = new ThrottlingPolicy();
                model.Properties.Add("Throttling.Policy", policy);
                model.Filters.Add(new ThrottlingAuthorizationFilterFactory(policy));
            }
            else
            {
                policy = (ThrottlingPolicy)policyObj;
            }

            policy.AddPolicy(new IPLimitRatePolicy(_limit, TimeSpan.FromSeconds(_window), Sliding));
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UserThrottling : Attribute, IActionModelConvention
    {
        private readonly long _authenticatedLimit;

        private readonly long _authenticatedWindow;

        private readonly long _unauthenticatedLimit;

        private readonly long _unauthenticatedWindow;

        public UserThrottling(long authenticatedLimit, long authenticatedWindow, long unauthenticatedLimit, long unauthenticatedWindow)
        {
            _authenticatedLimit = authenticatedLimit;
            _authenticatedWindow = authenticatedWindow;
            _unauthenticatedLimit = unauthenticatedLimit;
            _unauthenticatedWindow = unauthenticatedWindow;
        }

        public UserThrottling(long authenticatedLimit, long authenticatedWindow)
            : this(authenticatedLimit, authenticatedWindow, 0L, 0L)
        {
        }

        public bool Sliding { get; set; }

        public void Apply(ActionModel model)
        {
            // TODO : Add a category based on Url template
            // TODO : Test with areas
            object policyObj;
            ThrottlingPolicy policy;
            if (!model.Properties.TryGetValue("Throttling.Policy", out policyObj))
            {
                policy = new ThrottlingPolicy();
                model.Properties.Add("Throttling.Policy", policy);
                model.Filters.Add(new ThrottlingAuthorizationFilterFactory(policy));
            }
            else
            {
                policy = (ThrottlingPolicy)policyObj;
            }

            policy.AddPolicy(new UserLimitRatePolicy(_authenticatedLimit, TimeSpan.FromSeconds(_authenticatedWindow), _unauthenticatedLimit, TimeSpan.FromSeconds(_unauthenticatedWindow), Sliding));
        }
    }
}