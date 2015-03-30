using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
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

    public class IPThrottling : Attribute, IActionModelConvention
    {
        public long Limit { get; set; }

        public int Window { get; set; }

        public bool Sliding { get; set; }

        public void Apply(ActionModel model)
        {
            // TODO : Add a category based on Url template
            // TODO : Test with areas
    //        model.Filters.Add(new ThrottlingAuthorizationFilterFactory(_policyName));
        }
    }
}